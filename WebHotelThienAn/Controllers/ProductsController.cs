using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebHotelThienAn.Models;

namespace WebHotelThienAn.Controllers
{
    public class ProductsController : Controller
    {
        MyDataContextDataContext data = new MyDataContextDataContext();
        public ActionResult AllProducts()
        {
            var all_products = LoadData();
            return View(all_products);
        }

        public List<Home> LoadData()
        {
            List<Home> lsthome = new List<Home>();
            var LoaiPhong = data.LoaiPhongs.Where(p => p.TrangThai == true).ToList();
            var khuVucs = data.KhuVucs.ToList();
            foreach (var item in LoaiPhong)
            {
                Home home = new Home();
                home.IDHome = item.IDLoaiPhong;
                home.Ten = item.TenLoaiPhong;
                home.soluong = item.SoLuongPhong;
                home.url = item.AnhDaiDien;
                home.l = "LoaiPhong";
                lsthome.Add(home);
            }
            foreach (var item in khuVucs)
            {
                Home home = new Home();
                home.IDHome = item.IDKhuVuc;
                home.Ten = item.TenKV;
                home.soluong = item.SoLuongPhong;
                home.url = item.AnhDaiDien;
                home.l = "KhuVuc";
                lsthome.Add(home);
            }
            return lsthome;
        }
        //===================List Phong======================
        public ActionResult ProductWithArea(int? idKhuVuc, int? idLoaiPhong, int? page, string datethue, string datetra) // show Phong theo mã nhà cung cấp
        {

            var listProduct = data.Phongs.Where(p => p.IDKhuVuc.ToString().Contains(idKhuVuc.ToString()) && p.IDLoaiPhong.ToString().Contains(idLoaiPhong.ToString()) && p.TrangThai == true).ToList();
            KhuVuc kv = data.KhuVucs.Where(p => p.IDKhuVuc == idKhuVuc).FirstOrDefault();
            ViewBag.datethue = datethue;
            ViewBag.datetra = datetra;
            ViewBag.SoLuongPhong = kv.SoLuongPhong;
            ViewBag.TenKV = kv.TenKV;
            ViewBag.idKhuVuc = idKhuVuc;
            //===================phân trang======================
            int pageSize = 8; // mỗi trang 8 sản phẩm
            int pageNum = (page ?? 1); // nếu page = null => pageNum = 1
            return View(listProduct.ToPagedList(pageNum, pageSize));
        }

        public ActionResult SelectArea(int? idLoaiPhong, int? page, string datethue, string datetra) // show Phong theo mã nhà cung cấp
        {

            var listProduct = data.KhuVucs.ToList();
            ViewBag.datethue = datethue;
            ViewBag.datetra = datetra;
            LoaiPhong lp = data.LoaiPhongs.Where(p => p.IDLoaiPhong == idLoaiPhong && p.TrangThai == true).FirstOrDefault();
            if (lp != null)
            {
                ViewBag.idLoaiPhong = lp.IDLoaiPhong;
                ViewBag.TenKV = lp.TenLoaiPhong;
            }
            return View(listProduct);
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
        [HttpGet]
        public ActionResult ProductDetail(int? id, string datethue, string datetra)
        {
            var D_SanPham = data.Phongs.Where(m => m.MaPhong == id).First();
            ViewBag.datethue = datethue;
            ViewBag.datetra = datetra;
            Session["idphong"] = id;
            return View(D_SanPham);
        }
        [HttpPost]
        public ActionResult ProductDetail(FormCollection f)
        {

            var songuoi = f["songuoi"];
            var DateThue = f["dateThue"];
            var DateTra = f["dateTra"];
            var id = Session["idphong"];
            int id1 = (int)id;
            var D_SanPham = data.Phongs.Where(m => m.MaPhong == id1).First();

            ViewBag.datethue = DateThue;
            ViewBag.datetra = DateTra;

            if (String.IsNullOrEmpty(DateThue))
                ViewBag.NgayThue = "! Ngày Thuê Trống";
            else if (String.IsNullOrEmpty(DateTra))
                ViewBag.NgaySinh = "! Ngày Trả Trống";
            else if (String.IsNullOrEmpty(songuoi))
                ViewBag.songuoi = "! Số Người Trống";
            else
            {
                return RedirectToAction("Checkout", "Booking", new { idPhong = id1, datethue = DateThue, datetra = DateTra });
            }

            return View(D_SanPham);
        }
        public ActionResult ShowLoaiPhong(int? id)
        {
            var product = data.LoaiPhongs.ToList();

            ViewBag.idKhuVuc = id;

            return PartialView(product);
        }
        public ActionResult ShowTietIchPhong(int? MaP)
        {
            var product = data.ChiTietTienNghis.Where(p => p.MaPhong == MaP && p.TinhTrang == true).ToList();

            ViewBag.MaP = MaP;

            return PartialView(product);
        }
        public ActionResult ShowTietAnhPhong(int? MaP)
        {
            var product = data.AnhPhongs.Where(p => p.MaPhong == MaP).ToList();

            ViewBag.MaP = MaP;

            return PartialView(product);
        }
        public ActionResult SearchProduct(int? page, FormCollection f)
        {
            var Location = f["Location"];
            var DateThue = f["DateThue"];
            var DateTra = f["DateTra"];

            if (!String.IsNullOrEmpty(Location))
            {
                KhuVuc kv = data.KhuVucs.Where(p => p.TenKV == Location).FirstOrDefault();
                if (kv != null && !String.IsNullOrEmpty(DateThue) || !String.IsNullOrEmpty(DateTra))
                {
                    return RedirectToAction("ProductWithArea", "Products", new { idKhuVuc = kv.IDKhuVuc, datethue = DateThue, datetra = DateTra });
                }

                else
                {
                    return RedirectToAction("ProductWithArea", "Products", new { idKhuVuc = kv.IDKhuVuc });
                }
            }
            else if (!String.IsNullOrEmpty(DateThue) || !String.IsNullOrEmpty(DateTra))
            {
                return RedirectToAction("SelectArea", "Products", new { datethue = DateThue, datetra = DateTra });
            }
            else
            {
                return RedirectToAction("SelectArea", "Products");
            }

            
        }
    }
}