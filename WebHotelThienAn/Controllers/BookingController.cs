using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebHotelThienAn.Models;

namespace WebHotelThienAn.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        MyDataContextDataContext data = new MyDataContextDataContext();


        public int GetSoNgayThue(DateTime ngaythue, DateTime ngaytra)
        {
            int SoNgay;
            SoNgay = ngaytra.Day - ngaythue.Day;
            return SoNgay;
        }

        public float GetTongTien(int? idphong, DateTime datethue, DateTime datetra)
        {
            float TongTien = 0;
            Phong phong = data.Phongs.Where(p => p.MaPhong == idphong).First();
            int songay = GetSoNgayThue(datethue, datetra);
            TongTien = (float)(songay * phong.GiaPhong);
            return TongTien;
        }

        [HttpGet]
        public ActionResult Checkout(int? idPhong, string datethue, string datetra)
        {
            DateTime ngaythue =DateTime.Parse(datethue);
            DateTime ngaytra =DateTime.Parse(datetra);
            ViewBag.SoNgayThue = GetSoNgayThue(ngaythue, ngaytra);
            ViewBag.datethue = ngaythue.ToString("dd-MM-yyyy");
            ViewBag.datetra = ngaytra.ToString("dd-MM-yyyy");
            ViewBag.TongTien = GetTongTien(idPhong, ngaythue, ngaytra);

            Session["ngayThue"] = datethue;
            Session["ngayTra"] = datetra;
            Session["idphong"] = idPhong;
            if (Session["Accouts"] == null || Session["Accouts"].ToString() == "")
            {
                return RedirectToAction("Login", "Accounts");
            }
      
            Phong phong = data.Phongs.Where(p => p.MaPhong == idPhong).First();
            return View(phong);

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
        public ActionResult Checkout(FormCollection form)
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login", "Accounts");
            string email = Session["Email"].ToString();
            TaiKhoan tk = data.TaiKhoans.FirstOrDefault(p => p.Email == email);
            string Name = form["Hoten"];
            string SDT = form["Phone"];
            string NgaySinh = form["Birthday"];
    
            string DiaChi = form["location"];
            string email1 = form["email"];

            var datethue = Session["ngayThue"];
            var datetra = Session["ngayTra"];

            var idphong = Session["idphong"];
            DateTime ngaythue = DateTime.Parse((string)datethue);
            DateTime ngaytra = DateTime.Parse((string)datetra);
            if (String.IsNullOrEmpty(Name))
                ViewBag.Name = "! Họ Và Tên Không Được Để Trống";
            else if (String.IsNullOrEmpty(SDT))
                ViewBag.SDT = "! Số Điện Thoại Không Được Để Trống";
            else if (String.IsNullOrEmpty(email1))
                ViewBag.email = "! email Không Được Để Trống";
            else if (String.IsNullOrEmpty(DiaChi))
                ViewBag.DiaChi = "! Địa Chỉ Không Được Để Trống";
            else if (String.IsNullOrEmpty(NgaySinh))
                ViewBag.NgaySinh = "! Ngày Sinh Không Được Để Trống";
            else if (SDT.Length < 10 || SDT.Length > 11)
                ViewBag.SDT = "! Bạn Phải Nhập Số Điện Thoại Chính Xác";
            else
            {
                HoaDon hd = new HoaDon();
                hd.IDTaiKhoan = tk.IDTaiKhoan;
                hd.NgayLap = DateTime.Now;
                var tinhtrang = form["slt-1"];
                hd.TinhTrang = int.Parse(tinhtrang);
                hd.NgayThue = ngaythue;
                hd.MaPhong = (int?)idphong;
                hd.NgayTra = ngaytra;
                hd.TongTien = GetTongTien((int?)idphong, ngaythue, ngaytra);
                data.HoaDons.InsertOnSubmit(hd);
                data.SubmitChanges();
                _SendMail(tk.Email, "Đặt Phòng Thành Công", "Bạn Đã Đặt Phòng Thành Công lúc " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm"));
                return RedirectToAction("NotifForm", "Accounts", new { title = "Thanh Toán Thành Công", msg = "Vui lòng check mail để xác nhận Mail!" });
            }

           
            
            ViewBag.SoNgayThue = GetSoNgayThue(ngaythue, ngaytra);
            ViewBag.datethue = ngaythue.ToString("dd-MM-yyyy");
            ViewBag.datetra = ngaytra.ToString("dd-MM-yyyy");
            ViewBag.TongTien = GetTongTien((int?)idphong, ngaythue, ngaytra);
            Phong phong = data.Phongs.Where(p => p.MaPhong == (int)idphong).First();
            return View(phong);

        }
    }
}