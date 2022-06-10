using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebHotelThienAn.Models;

namespace WebHotelThienAn.Controllers
{
    public class AccountsController : Controller
    {
        MyDataContextDataContext db = new MyDataContextDataContext();
        EmailAddressAttribute foo = new EmailAddressAttribute();
        public const string clientId = "493593590669-mnehgoe6fclfnoe4ccq1voc5h9nehsfo.apps.googleusercontent.com";
        public const string clientSecret = "1EtbCsURRhXJSrHnGxKdNiRm";
        public static string MD5Hash(string input) //MD5
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
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
        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Accounts");
        }
        [HttpGet]
        public ActionResult Login(string thongbao)
        {
            if (Session["Email"] != null)
                return RedirectToAction("AllProducts", "Products");
            ViewBag.ThongBao = thongbao;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string email = form["email"];
            string mk = form["matkhau"];
            if (String.IsNullOrEmpty(email))
            {
                ViewBag.ThongBao = "Email Không Được Để Trống !";
            }
            else if (String.IsNullOrEmpty(mk))
            {
                ViewBag.ThongBao = "Mật Khẩu Không Được Để Trống !";
            }
            else if (mk.Length > 18)
            {
                ViewBag.ThongBao = "Mật Khẩu Không Được Quá 18 Ký Tự !";
            }
            else
            {
                string md5Pass = MD5Hash(mk);
                TaiKhoan nv = db.TaiKhoans.FirstOrDefault(p => p.Email == email && p.MatKhau == md5Pass);
                if (nv != null)
                {
                    if (nv.maTinhTrang == "block")
                    {
                        return RedirectToAction("NotifForm", "Accounts", new { title = "Tài Khoản Chưa Xác Thực", msg = "Tài Khoản Bạn Chưa Được Xác Thực. Vui Lòng Kiểm Tra Mail" });
                    }
                    Session["ID"] = nv.IDTaiKhoan;
                    Session["Email"] = nv.Email;
                    Session["Name"] = nv.HoTen;
                    Session["SDT"] = nv.SDT;
                    Session["DiaChi"] = nv.DiaChi;
                    if (String.IsNullOrEmpty(nv.MatKhau) || String.IsNullOrEmpty(nv.SDT) || String.IsNullOrEmpty(nv.DiaChi) || String.IsNullOrEmpty(nv.NgaySinh.ToString()) || String.IsNullOrEmpty(nv.GioiTinh.ToString()))
                        return RedirectToAction("UpdateInfo", "Accounts");
                    else
                    {
                        Session["Accouts"] = nv;
                        Session["ID"] = nv.IDTaiKhoan;
                        Session["Email"] = nv.Email;
                        Session["Name"] = nv.HoTen;
                        Session["SDT"] = nv.SDT;
                        Session["DiaChi"] = nv.DiaChi;
                        if (nv.Quyen == true)//Admin
                        {
                            ViewBag.ThongBao = "Đăng nhập thành công admin";
                            return RedirectToAction("ThongKe", "Admin");
                        }
                        if (nv.Quyen == false || nv.Quyen == null)
                        {
                            ViewBag.ThongBao = "Đăng nhập thành công";
                            return RedirectToAction("AllProducts", "Products");
                        }
                    }

                }
                else
                {
                    ViewBag.ThongBao = "! Tài Khoản Và Mật Khẩu Không Hợp Lệ!";
                }
            }
            return RedirectToAction("Login", "Accounts", new { thongbao = ViewBag.ThongBao });
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            string name = form["name"];
            string email = form["email"];
            string pass = form["password"];
            string repass = form["repassword"];

            if (String.IsNullOrEmpty(name))
            {
                ViewBag.ThongBao = "! Họ và Tên Không Được Để Trống";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewBag.ThongBao = "! Email Không Được Để Trống";
            }
            else if (!foo.IsValid(email))
            {
                ViewBag.ThongBao = "! Email Không Hợp Lệ";
            }
            else if (String.IsNullOrEmpty(pass))
            {
                ViewBag.ThongBao = "! Mật Khẩu Không Được Để Trống";
            }
            else if (String.IsNullOrEmpty(repass))
            {
                ViewBag.ThongBao = "! Mật Khẩu Không Được Để Trống";
            }
            else if (pass.Length > 18)
            {
                ViewBag.ThongBao = "! Mật Khẩu Không Được Quá 18 Ký Tự";

            }
            else if (repass.Length > 18)
            {
                ViewBag.ThongBao = "! Mật Khẩu Không Được Quá 18 Ký Tự";
            }
            else if (repass != pass)
            {
                ViewBag.ThongBao = "! Mật Khẩu Không Giống Nhau";
            }
            else
            {
                TaiKhoan e = db.TaiKhoans.FirstOrDefault(p => p.Email == email);
                if (e != null)
                {
                    ViewBag.ThongBao = "! Email Này Đã Tồn Tại";
                }
                else
                {
                    string host = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");
                    string maXN = MD5Hash(DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));

                    TaiKhoan user = new TaiKhoan();
                    user.Email = email;
                    user.HoTen = name;
                    user.MatKhau = MD5Hash(pass);
                    user.maTinhTrang = "block";
                    db.TaiKhoans.InsertOnSubmit(user);
                    db.SubmitChanges();

                    ResetPass reset = new ResetPass();
                    reset.maRS = maXN;
                    reset.ID_NguoiDung = user.IDTaiKhoan;
                    reset.ThoiHan = "1";
                    db.ResetPasses.InsertOnSubmit(reset);
                    db.SubmitChanges();
                    _SendMail(email, "Xác Nhận Tài Khoản StoreFZF", "Link Xác Nhận Tài Khoản Của Bạn Là: " + host + "/Accounts/ActiveUser?token=" + maXN);
                    return RedirectToAction("NotifForm", "Accounts", new { title = "Tạo Tài Khoản Thành Công", msg = "Tài khoản của bạn đã được tạo thành công. Vui lòng check mail để xác nhận Mail!" });
                }
            }
            return RedirectToAction("Register", "Accounts", new { thongbao = ViewBag.ThongBao });
        }



        [HttpGet]
        public ActionResult Forget()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Forget(FormCollection form)
        {
            string email = form["Email"].ToString();
            if (String.IsNullOrEmpty(email))
            {
                ViewBag.MatKhau = "! Email Không Được Để Trống";
            }
            else
            {
                TaiKhoan nv = db.TaiKhoans.FirstOrDefault(p => p.Email == email);
                if (nv != null)
                {
                    string host = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");
                    string thoiHan = DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss");
                    string maRS = MD5Hash(thoiHan);
                    ResetPass reset = new ResetPass();
                    reset.maRS = maRS;
                    reset.ID_NguoiDung = nv.IDTaiKhoan;
                    reset.ThoiHan = thoiHan;
                    db.ResetPasses.InsertOnSubmit(reset);
                    db.SubmitChanges();
                    _SendMail(email, "Khôi Phục Mật Khẩu StoreFZF", "Link Khôi Phục Mật Khẩu Của Bạn Là: " + host + "/Accounts/ResetPass?token=" + maRS);
                    return RedirectToAction("NotifForm", "Accounts", new { title = "Yêu Cầu Thành Công", msg = "Chúng tôi đã gửi link khôi phục về Email: " + email + " của bạn." });
                }
                else
                {
                    ViewBag.ThongBao = "Tài Khoản hoặc Email không hợp lệ!!";
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult NotifForm(string title, string msg)
        {
            ViewBag.TitleH = title;
            ViewBag.Msg = msg;
            return View();
        }

        [HttpGet]
        public ActionResult ResetPass(string token)
        {
            ResetPass reset = db.ResetPasses.FirstOrDefault(p => p.maRS == token);
            if (reset == null)
                return RedirectToAction("NotifForm", "Accounts", new { title = "Link Hết Hạn", msg = "Chúng tôi nhận thấy link của bạn đã hết hạn. Vui lòng tạo link quên mật khẩu mới!" });
            long ThoiHan = long.Parse(reset.ThoiHan);
            long Now = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (Now >= ThoiHan)
                return RedirectToAction("NotifForm", "Accounts", new { title = "Link Hết Hạn", msg = "Chúng tôi nhận thấy link của bạn đã hết hạn. Vui lòng tạo link quên mật khẩu mới!" });
            if (ThoiHan == 1)
                return RedirectToAction("NotifForm", "Accounts", new { title = "Link Không Hợp Lệ", msg = "Chúng tôi nhận thấy link của bạn không hợp lệ. Vui lòng tạo link quên mật khẩu mới!" });

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetPass(FormCollection form)
        {
            Console.Write(form);
            string maRS = form["token"];
            string Pass = form["password"];
            string rePass = form["repassword"];
            if (String.IsNullOrEmpty(maRS))
            {
                return RedirectToAction("NotifForm", "Accounts", new { title = "Lỗi Link", msg = "Chúng tôi đã phát hiện lỗi." });
            }
            else if (String.IsNullOrEmpty(Pass))
            {
                ViewBag.Pass = "! Không Được Để Mật Khẩu Trống";
            }
            else if (String.IsNullOrEmpty(rePass))
            {
                ViewBag.RePass = "! Không Được Để Mật Khẩu Trống";
            }
            else if (Pass != rePass)
            {
                ViewBag.RePass = "! Mật Khẩu Không Khớp";
            }
            else
            {
                ResetPass rs = db.ResetPasses.FirstOrDefault(p => p.maRS == maRS);
                if (rs != null)
                {
                    string email = rs.TaiKhoan.Email;
                    rs.TaiKhoan.MatKhau = MD5Hash(Pass);
                    List<ResetPass> ListRS = db.ResetPasses.Where(p => p.ID_NguoiDung == rs.ID_NguoiDung).ToList();
                    foreach (var item in ListRS)
                    {
                        if (item.ThoiHan != "1")
                            db.ResetPasses.DeleteOnSubmit(item);
                    }
                    db.SubmitChanges();
                    _SendMail(email, "Đổi Mật Khẩu Thành Công", "Mật Khẩu của " + email + " đã được đổi thành công lúc " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm"));
                    return RedirectToAction("NotifForm", "Accounts", new { title = "Yêu Cầu Thành Công", msg = "Mất Khẩu của bạn đã được đổi thành công!" });
                }
                else
                {
                    return RedirectToAction("NotifForm", "Accounts", new { title = "Link Hết Hạn", msg = "Chúng tôi nhận thấy link của bạn đã hết hạn. Vui lòng tạo link quên mật khẩu mới!" });
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Accounts");
        }

        [HttpGet]
        public ActionResult ActiveUser(string token)
        {
            ResetPass reset = db.ResetPasses.FirstOrDefault(p => p.maRS == token);
            if (reset == null)
                return RedirectToAction("NotifForm", "Accounts", new { title = "Link Hết Hạn", msg = "Chúng tôi nhận thấy link của bạn đã hết hạn. Vui lòng tạo link quên mật khẩu mới!" });
            long ThoiHan = long.Parse(reset.ThoiHan);
            if (ThoiHan != 1)
                return RedirectToAction("NotifForm", "Accounts", new { title = "Link Lỗi", msg = "Chúng tôi nhận thấy link của bạn đã bị lỗi. Vui lòng tạo link quên mật khẩu mới!" });
            reset.TaiKhoan.maTinhTrang = "active";
            db.ResetPasses.DeleteOnSubmit(reset);
            db.SubmitChanges();
            return RedirectToAction("NotifForm", "Accounts", new { title = "Xác Minh Mail Thành Công", msg = "Bạn Đã Xác Minh Địa Chỉ Thành Công!" });
        }


        [HttpGet]
        public ActionResult UpdateInfo()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login", "Accounts");
            string email = Session["Email"].ToString();
            TaiKhoan nd = db.TaiKhoans.FirstOrDefault(p => p.Email == email);
            if (nd == null) return RedirectToAction("Login", "Accounts");
            return View(nd);
        }

        [HttpPost]
        public ActionResult UpdateInfo(FormCollection form)
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login", "Accounts");
            string email = Session["Email"].ToString();
            TaiKhoan nd = db.TaiKhoans.FirstOrDefault(p => p.Email == email);
            string pass = form["pass"];
            string Repass = form["Repass"];
            string Name = form["Name"];
            string SDT = form["SDT"];
            string NgaySinh = form["NgaySinh"];
            string GioiTinh = form["GioiTinh"];
            string DiaChi = form["DiaChi"];

            if (pass == "" || Repass == "")
                ViewBag.Pass = "! Mật Khẩu Không Được Để Trống";
            else if (pass != Repass)
                ViewBag.RePass = "! Mật Khẩu Không Khớp";
            else if (String.IsNullOrEmpty(Name))
                ViewBag.Name = "! Họ Và Tên Không Được Để Trống";
            else if (String.IsNullOrEmpty(SDT))
                ViewBag.SDT = "! Số Điện Thoại Không Được Để Trống";
            else if (String.IsNullOrEmpty(NgaySinh))
                ViewBag.NgaySinh = "! Ngày Sinh Không Được Để Trống";
            else if (String.IsNullOrEmpty(GioiTinh))
                ViewBag.GioiTinh = "! Bạn Phải Chọn Giới Tính";
            else if (String.IsNullOrEmpty(DiaChi))
                ViewBag.DiaChi = "! Địa Chỉ Không Được Để Trống";
            else if (SDT.Length < 10 || SDT.Length > 11)
                ViewBag.SDT = "! Bạn Phải Nhập Số Điện Thoại Chính Xác";
            else
            {
                if (nd != null)
                {
                    if (pass != null && pass != "") nd.MatKhau = MD5Hash(pass);
                    nd.HoTen = Name;
                    Session["Name"] = Name;
                    nd.SDT = SDT;
                    nd.NgaySinh = DateTime.Parse(NgaySinh);
                    nd.GioiTinh = GioiTinh;
                    nd.DiaChi = DiaChi;
                    db.SubmitChanges();
                    Session["SDT"] = nd.SDT;
                    Session["DiaChi"] = nd.DiaChi;
                    _SendMail(nd.Email, "Thay Đổi Thông Tin", "Bạn Đã Cập Nhật Thông Tin Thành Công lúc " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm"));
                    return RedirectToAction("NotifForm", "Accounts", new { title = "Cập Nhật Thông Tin Thành Công", msg = "Bạn Đã Cập Nhật Thông Tin Thành Công!!!" });
                }
                else
                {
                    return RedirectToAction("NotifForm", "Accounts", new { title = "Có Lỗi Khi Cập Nhật Thông Tin", msg = "Chúng tôi phát hiện lỗi khi cập nhật thông tin. Vui Lòng đăng nhập lại!!!" });
                }
            }
            return View(nd);
        }
        [HttpGet]
        public ActionResult ChangePass()
        {
            if (Session["Email"] == null) return RedirectToAction("Login", "Accounts");
            return View();
        }

        [HttpPost]
        public ActionResult ChangePass(FormCollection form)
        {
            if (Session["Email"] == null) return RedirectToAction("Login", "Accounts");
            string passOld = form["passOld"];
            string passNew = form["passNew"];
            string passNew2 = form["passNew2"];
            if (String.IsNullOrEmpty(passOld) || String.IsNullOrEmpty(passNew) || String.IsNullOrEmpty(passNew2))
            {
                ViewBag.ThongBao = "Vui Lòng Nhập Đầy Đủ Thông Tin";
            }
            else if (passNew != passNew2)
            {
                ViewBag.ThongBao = "Mật Khẩu Mới Không Trùng";
            }
            else
            {
                string md5Pass = MD5Hash(passOld);
                string email = Session["Email"].ToString();
                TaiKhoan nd = db.TaiKhoans.FirstOrDefault(p => p.Email == email && p.MatKhau == md5Pass);
                if (nd == null)
                {
                    ViewBag.ThongBao = "Mật Khẩu Cũ Không Đúng";
                }
                else
                {
                    nd.MatKhau = MD5Hash(passNew);
                    db.SubmitChanges();
                    Session.Clear();
                    return RedirectToAction("NotifForm", "Accounts", new { title = "Đổi Mật Khẩu Thành Công", msg = "Bạn Đã Đổi Mật Khẩu Thành Công. Vui Lòng Đăng Nhập Lại!!!" });
                }
            }

            return View();
        }
    }
}