using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DOAN_COSO.Extension;
using PayPal.Api;
using System.Net;
using System.Web.Script.Serialization;
using WebHotelThienAn.Models;
using System.Net.Mail;

namespace DOAN_COSO.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        MyDataContextDataContext data = new MyDataContextDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PaymentWithMomo()
        {
            //MOMOIQA420180417
            //SvDmj2cOTYZmQQ3H
            //PPuDXq1KowPT1ftR8DvlQTHhC03aul17
            
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOBJFW20220409";
            string accessKey = "zWvuzuPQ0Uw2oybj";
            string serectkey = "TZvJHl5lIl7oP28cNefAvHvaGhyvdnFu";
            string orderInfo = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string returnUrl = "https://localhost:44386/GioHang/Success";
            string notifyurl = "https://localhost:44386/GioHang/Success"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
            string tt = 100000.ToString();
            string amount = tt;
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        private void _SendMail(string gmail, string subject, string msg)
        {
            try
            {
                MailMessage message = new MailMessage("phuclongofficial@gmail.com", gmail);
                SmtpClient mailclient = new SmtpClient("smtp.gmail.com", 587);
                mailclient.EnableSsl = true;
                mailclient.Credentials = new NetworkCredential("phuclongofficial@gmail.com", "sftxjbdaqrnpahwq");
                message.Subject = subject;
                message.Body = msg;
                mailclient.Send(message);
                //Program.Alert("Mật khẩu khôi phục đã gửi qua Mail", Form_Alert.enmType.Success);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            { }
        }
        private PayPal.Api.Payment payment;

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Payment/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("NotifForm", "Accounts", new { title = "Thanh Toán Không Thành Công", msg = "Lỗi" });
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("NotifForm", "Accounts", new { title = "Thanh Toán Không Thành Công", msg = ex.Message });
    
            }
            if (Session["Email"] == null)
                return RedirectToAction("Login", "Accounts");
            string email = Session["Email"].ToString();
            TaiKhoan tk = data.TaiKhoans.FirstOrDefault(p => p.Email == email);
            var hd = new HoaDon();
            hd.IDTaiKhoan = tk.IDTaiKhoan;
            hd.NgayLap = DateTime.Now;
            hoadon hd1 = (hoadon)Session["hoadon"];
            hd.TinhTrang = hd1.tintrang;
            hd.NgayThue = hd1.ngaythue;
            hd.MaPhong = hd1.idphong;
            hd.NgayTra = hd1.ngaytra;
            hd.TongTien = hd1.tongtien;
            data.HoaDons.InsertOnSubmit(hd);
            data.SubmitChanges();
            _SendMail(tk.Email, "Đặt Phòng Thành Công", "Bạn Đã Đặt Phòng Thành Công lúc " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm"));
            return RedirectToAction("NotifForm", "Accounts", new { title = "Thanh Toán Thành Công", msg = "Vui lòng check mail để xác nhận Mail!" });
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apicontext, string redirectURl)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            if (Session["Phongdat"] != null)
            {
                var d = GetCurrencyExchange("VND", "USD");
                Phongdat phong = (Phongdat)Session["Phongdat"];

                    decimal p = Math.Round(int.Parse(phong.phong.GiaPhong.ToString()) * d, 0);
                    itemList.items.Add(new Item()
                    {
                        name  = phong.phong.TenPhong,
                        currency = "USD",
                        price = p.ToString(),
                        quantity = phong.SoNgay.ToString(),
                        sku = "sku"
                    });;

                var payer = new Payer()
                {
                    payment_method = "paypal"
                };

                var redirUrl = new RedirectUrls()
                {
                    cancel_url = redirectURl + "&Cancel=true",
                    return_url = redirectURl
                };
  
                
                var details = new Details()
                {
                    tax = "0",
                    shipping = "0",
                    
                    subtotal = itemList.items.Sum(x => int.Parse(x.price) * int.Parse(x.quantity) ).ToString()
                };

                var amount = new Amount()
                {
                    currency = "USD",
                    total = details.subtotal,
                    details = details
                };

                var transactionList = new List<Transaction>();
                transactionList.Add(new Transaction()
                {
                    description = "Transaction Description",
                    invoice_number = Convert.ToString((new Random()).Next(100000)),
                    amount = amount,
                    item_list = itemList
                });

                this.payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrl
                };
            }

            return this.payment.Create(apicontext);
        }

        public Decimal GetCurrencyExchange(String localCurrency, String foreignCurrency)
        {
            var code = $"{localCurrency}_{foreignCurrency}";
            var newRate = FetchSerializedData(code);
            return newRate;
        }

        private Decimal FetchSerializedData(String code)
        {
            var url = "https://free.currconv.com/api/v7/convert?q=" + code + "&compact=y&apiKey=7cf3529b5d3edf9fa798";
            var webClient = new WebClient();
            var jsonData = String.Empty;

            var conversionRate = 1.0m;
            try
            {
                jsonData = webClient.DownloadString(url);
                var jsonObject = new JavaScriptSerializer().Deserialize<Dictionary<string, Dictionary<string, decimal>>>(jsonData);
                var result = jsonObject[code];
                conversionRate = result["val"];

            }
            catch (Exception) { }

            return conversionRate;
        }
    }
}