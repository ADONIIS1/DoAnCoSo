using System;
using System.Collections.Generic;
using System.Linq;
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
            var LoaiPhong = data.LoaiPhongs.ToList();
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
        public ActionResult ProductWithArea(int? idKhuVuc, int? idLoaiPhong, int? page) // show Phong theo mã nhà cung cấp
        {
           
            var listProduct = data.Phongs.Where(p => p.IDKhuVuc.ToString().Contains(idKhuVuc.ToString()) && p.IDLoaiPhong.ToString().Contains(idLoaiPhong.ToString()) ).ToList();
            KhuVuc kv = data.KhuVucs.Where(p => p.IDKhuVuc == idKhuVuc).FirstOrDefault();
      
            ViewBag.SoLuongPhong = kv.SoLuongPhong;
            ViewBag.TenKV = kv.TenKV;
            ViewBag.idKhuVuc = idKhuVuc;


            //===================phân trang======================
            int pageSize = 8; // mỗi trang 8 sản phẩm
            int pageNum = (page ?? 1); // nếu page = null => pageNum = 1
            return View(listProduct.ToPagedList(pageNum, pageSize));
        }
        public ActionResult SelectArea( int? idLoaiPhong, int? page) // show Phong theo mã nhà cung cấp
        {

            var listProduct = data.KhuVucs.ToList();

            LoaiPhong lp = data.LoaiPhongs.Where(p => p.IDLoaiPhong == idLoaiPhong).FirstOrDefault();
            if(lp != null)
            {
                ViewBag.idLoaiPhong = lp.IDLoaiPhong;
                ViewBag.TenKV = lp.TenLoaiPhong;
            }
            return View(listProduct);
        }
        public ActionResult ProductDetail(int? id)
        {
            var D_SanPham = data.Phongs.Where(m => m.MaPhong == id).First();
            return View(D_SanPham);
        }
        public ActionResult ShowLoaiPhong(int? id) // show navigation chọn sản phẩm theo thương hiệu (nhà cung cấp)
        {
            var product = data.LoaiPhongs.ToList();

            ViewBag.idKhuVuc = id;

            return PartialView(product);
        }
        public ActionResult ShowTietIchPhong(int? MaP) // show navigation chọn sản phẩm theo thương hiệu (nhà cung cấp)
        {
            var product = data.ChiTietTienNghis.Where(p=>p.MaPhong == MaP && p.TinhTrang == true).ToList();

            ViewBag.MaP = MaP;

            return PartialView(product);
        }
    }
}