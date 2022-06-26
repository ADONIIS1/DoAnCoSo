using System;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using WebHotelThienAn.Models;
using WebHotelThienAn.Controllers;

namespace DoAnCoSo.Controllers
{
    public class AdminController : BaseController
    {

        MyDataContextDataContext data = new MyDataContextDataContext();
        // GET: Admin

        #region Thống kê


        public ActionResult ThongKeDoanhThu()
        {
            return View();
        }
        public ActionResult GetTong(int? nam)
        {

            if (nam == null)
            {
                var query = data.HoaDons
                                      .GroupBy(n => new { thangs = n.NgayLap.Value })
                                      .Where(n => n.Key.thangs.Year == DateTime.Now.Year)
                                     .Select(s => new { months = s.Key.thangs.Month, count = s.Sum(w => w.TongTien) });
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = data.HoaDons
                                      .GroupBy(n => new { thangs = n.NgayLap.Value })
                                      .Where(n => n.Key.thangs.Year == nam)
                                     .Select(s => new { months = s.Key.thangs.Month, count = s.Sum(w => w.TongTien) });
                return Json(query, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Khu Vuc


        public ActionResult KhuVuc(int? page, string timkiem)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNum = page ?? 1;
            ViewBag.Keyword = timkiem;
            if (String.IsNullOrEmpty(timkiem))
            {
                var khuVucs = data.KhuVucs.OrderBy(m => m.IDKhuVuc);
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var khuVucs = data.KhuVucs.Where(n => n.TenKV.ToLower().Contains(timkiem)).OrderBy(m => m.IDKhuVuc);
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
        }

        public ActionResult ThemKhuVuc()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemKhuVuc(FormCollection collection, KhuVuc khuVuc)
        {
            var TenKhuVuc = collection["TenKV"];
            var TienDe = collection["TieuDe"];
            var AnhDaiDien = collection["AnhDaiDien"];

            if (string.IsNullOrEmpty(TenKhuVuc))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                khuVuc.TenKV = TenKhuVuc;
                khuVuc.SoLuongPhong = null;
                khuVuc.TieuDe = TienDe;
                khuVuc.AnhDaiDien = AnhDaiDien;
                data.KhuVucs.InsertOnSubmit(khuVuc);
                data.SubmitChanges();
                return RedirectToAction("KhuVuc");
            }
            return this.ThemKhuVuc();
        }
        public ActionResult SuaKhuVuc(int id)
        {
            var khuVuc = data.KhuVucs.First(n => n.IDKhuVuc == id);
            if (khuVuc == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            return View(khuVuc);
        }
        [HttpPost]
        public ActionResult SuaKhuVuc(int id, FormCollection collection)
        {
            var khuVuc = data.KhuVucs.First(n => n.IDKhuVuc == id);
            var TenKhuVuc = collection["TenKV"];
            var TienDe = collection["TieuDe"];
            var AnhDaiDien = collection["AnhDaiDien"];

            if (String.IsNullOrEmpty(TenKhuVuc))
            {
                ViewData["Loi1"] = "Tên khu vực không được để trống";
            }
            else
            {
                khuVuc.TenKV = TenKhuVuc;
                khuVuc.SoLuongPhong = null;
                khuVuc.TieuDe = TienDe;
                khuVuc.AnhDaiDien = AnhDaiDien;
                UpdateModel(khuVuc);
                data.SubmitChanges();
                return RedirectToAction("KhuVuc");
            }
            return this.SuaKhuVuc(id);
        }

        public ActionResult XoaKhuVuc(int id)
        {
            try
            {
                KhuVuc khuVuc = data.KhuVucs.SingleOrDefault(n => n.IDKhuVuc == id);
                if (khuVuc == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.KhuVucs.DeleteOnSubmit(khuVuc);
                data.SubmitChanges();
                return Json(new
                {
                    result = true
                        ,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = false
                        ,
                    Message = "Lỗi"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region LoaiPhong

        public ActionResult LoaiPhong(int? page, string timkiem)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNum = page ?? 1;
            ViewBag.Keyword = timkiem;
            if (String.IsNullOrEmpty(timkiem))
            {
                var khuVucs = data.LoaiPhongs.OrderBy(m => m.IDLoaiPhong);
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var khuVucs = data.LoaiPhongs.Where(n => n.TenLoaiPhong.ToLower().Contains(timkiem)).OrderBy(m => m.IDLoaiPhong);
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
        }

        public ActionResult ThemLoaiPhong()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemLoaiPhong(FormCollection collection, LoaiPhong loaiPhong)
        {
            var TenLoaiPhong = collection["TenLoaiPhong"];
            var AnhDaiDien = collection["AnhDaiDien"];

            if (string.IsNullOrEmpty(TenLoaiPhong))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                loaiPhong.TenLoaiPhong = TenLoaiPhong;
                loaiPhong.SoLuongPhong = null;
                loaiPhong.AnhDaiDien = AnhDaiDien;
                loaiPhong.TrangThai = true;
                data.LoaiPhongs.InsertOnSubmit(loaiPhong);
                data.SubmitChanges();
                return RedirectToAction("LoaiPhong");
            }
            return this.ThemLoaiPhong();
        }
        public ActionResult SuaLoaiPhong(int id)
        {
            var khuVuc = data.LoaiPhongs.First(n => n.IDLoaiPhong == id);
            if (khuVuc == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            return View(khuVuc);
        }
        [HttpPost]

        public ActionResult SuaLoaiPhong(int id, FormCollection collection)
        {
            var loaiPhong = data.LoaiPhongs.First(n => n.IDLoaiPhong == id);
            var TenLoaiPhong = collection["TenLoaiPhong"];
            var AnhDaiDien = collection["AnhDaiDien"];

            if (String.IsNullOrEmpty(TenLoaiPhong))
            {
                ViewData["Loi1"] = "Tên loại phòng không được để trống";
            }
            else
            {
                loaiPhong.TenLoaiPhong = TenLoaiPhong;
                loaiPhong.SoLuongPhong = null;
                loaiPhong.AnhDaiDien = AnhDaiDien;
                loaiPhong.TrangThai = true;
                UpdateModel(loaiPhong);
                data.SubmitChanges();
                return RedirectToAction("LoaiPhong");
            }
            return this.SuaLoaiPhong(id);
        }

        public ActionResult XoaLoaiPhong(int id)
        {
            try
            {
                LoaiPhong loaiPhong = data.LoaiPhongs.SingleOrDefault(n => n.IDLoaiPhong == id);
                if (loaiPhong == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.LoaiPhongs.DeleteOnSubmit(loaiPhong);
                data.SubmitChanges();
                return Json(new
                {
                    result = true
                        ,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = false
                        ,
                    Message = "Lỗi"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Kiểu phòng 

        public ActionResult KieuPhong(int? page, string timkiem)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNum = page ?? 1;
            ViewBag.Keyword = timkiem;
            if (String.IsNullOrEmpty(timkiem))
            {
                var khuVucs = data.KieuPhongs;
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var khuVucs = data.KieuPhongs.Where(n => n.TenKieuPhong.ToLower().Contains(timkiem));
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
        }

        public ActionResult ThemKieuPhong()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemKieuPhong(FormCollection collection, KieuPhong kieuPhong)
        {
            var TenKieuPhong = collection["TenKieuPhong"];

            if (string.IsNullOrEmpty(TenKieuPhong))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                kieuPhong.TenKieuPhong = TenKieuPhong;
                data.KieuPhongs.InsertOnSubmit(kieuPhong);
                data.SubmitChanges();
                return RedirectToAction("KieuPhong");
            }
            return this.ThemLoaiPhong();
        }
        public ActionResult SuaKieuPhong(int id)
        {
            var khuVuc = data.KieuPhongs.First(n => n.IDKieuPhong == id);
            if (khuVuc == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            return View(khuVuc);
        }
        [HttpPost]
        public ActionResult SuaKieuPhong(int id, FormCollection collection)
        {
            var loaiPhong = data.KieuPhongs.First(n => n.IDKieuPhong == id);
            var TenKieuPhong = collection["TenKieuPhong"];

            if (String.IsNullOrEmpty(TenKieuPhong))
            {
                ViewData["Loi1"] = "Tên kiểu phòng không được để trống";
            }
            else
            {
                loaiPhong.TenKieuPhong = TenKieuPhong;
                UpdateModel(loaiPhong);
                data.SubmitChanges();
                return RedirectToAction("KieuPhong");
            }
            return this.SuaKieuPhong(id);
        }

        public ActionResult XoaKieuPhong(int id)
        {
            try
            {
                KieuPhong kieuPhong = data.KieuPhongs.SingleOrDefault(n => n.IDKieuPhong == id);
                if (kieuPhong == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.KieuPhongs.DeleteOnSubmit(kieuPhong);
                data.SubmitChanges();
                return Json(new
                {
                    result = true
                        ,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = false
                        ,
                    Message = "Lỗi"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Tiện nghi

        public ActionResult TienNghi(int? page, string timkiem)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNum = page ?? 1;
            ViewBag.Keyword = timkiem;
            if (String.IsNullOrEmpty(timkiem))
            {
                var khuVucs = data.TienNghis.OrderBy(m => m.IDTN);
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var khuVucs = data.TienNghis.Where(n => n.TenTN.ToLower().Contains(timkiem)).OrderBy(m => m.IDTN);
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
        }

        public ActionResult ThemTienNghi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemTienNghi(FormCollection collection, TienNghi tienNghi)
        {
            var TenTN = collection["TenTN"];
            var AnhDaiDien = collection["AnhDaiDien"];

            if (string.IsNullOrEmpty(TenTN))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                tienNghi.TenTN = TenTN;
                tienNghi.AnhDaiDien = AnhDaiDien;
                data.TienNghis.InsertOnSubmit(tienNghi);
                data.SubmitChanges();
                return RedirectToAction("TienNghi");
            }
            return this.ThemLoaiPhong();
        }
        public ActionResult SuaTienNghi(int id)
        {
            var tienNghi = data.TienNghis.First(n => n.IDTN == id);
            if (tienNghi == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            return View(tienNghi);
        }
        [HttpPost]
        public ActionResult SuaTienNghi(int id, FormCollection collection)
        {
            var tienNghi = data.TienNghis.First(n => n.IDTN == id);
            var TenTN = collection["TenTN"];
            var AnhDaiDien = collection["AnhDaiDien"];

            if (String.IsNullOrEmpty(TenTN))
            {
                ViewData["Loi1"] = "Tên loại sản phẩm không được để trống";
            }
            else
            {
                tienNghi.TenTN = TenTN;
                tienNghi.AnhDaiDien = AnhDaiDien;
                UpdateModel(tienNghi);
                data.SubmitChanges();
                return RedirectToAction("TienNghi");
            }
            return this.SuaTienNghi(id);
        }

        public ActionResult XoaTienNghi(int id)
        {
            try
            {
                TienNghi kieuPhong = data.TienNghis.SingleOrDefault(n => n.IDTN == id);
                if (kieuPhong == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.TienNghis.DeleteOnSubmit(kieuPhong);
                data.SubmitChanges();
                return Json(new
                {
                    result = true
                        ,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = false
                        ,
                    Message = "Lỗi"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Phòng

        public ActionResult Phong(int? page, string tenphong, int? KhuVuc, int? LoaiPhong, int? KieuPhong, bool? TrangThai)
        {
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNum = page ?? 1;
            ViewBag.Keyword = tenphong;
            List<SelectListItem> a = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Value = "true",
                    Text = "Đang hoạt động"
                },new SelectListItem
                {
                    Value = "false",
                    Text = "Ngừng hoạt động"
                }
            };

            ViewBag.TrangThai = a;
            ViewBag.LoaiPhong = new SelectList((data.LoaiPhongs).ToList(), "IDLoaiPhong", "TenLoaiPhong");
            ViewBag.KhuVuc = new SelectList((data.KhuVucs).ToList(), "IDKhuVuc", "TenKV");
            ViewBag.KieuPhong = new SelectList((data.KieuPhongs).ToList(), "IDKieuPhong", "TenKieuPhong");


            if (String.IsNullOrEmpty(tenphong))
            {
                if (KhuVuc == null)
                {
                    if (LoaiPhong == null)
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs;
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                    else // có loại phòng
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDLoaiPhong == LoaiPhong);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDLoaiPhong == LoaiPhong);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                }
                else // co khu vuc
                {
                    if (LoaiPhong == null)
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong && n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong && n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                    else // có loại phòng
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc);
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                }
            }
            else // có tên phòng
            {
                if (KhuVuc == null)
                {
                    if (LoaiPhong == null)
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                    else // có loại phòng
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDLoaiPhong == LoaiPhong && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDLoaiPhong == LoaiPhong && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                }
                else // co khu vuc
                {
                    if (LoaiPhong == null)
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong && n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong && n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                    else // có loại phòng
                    {
                        if (KieuPhong == null)
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                        else // co kieu phong
                        {
                            if (TrangThai == null)
                            {
                                var phong = data.Phongs.Where(n => n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                            else
                            {
                                var phong = data.Phongs.Where(n => n.TrangThai == TrangThai && n.IDKieuPhong == KieuPhong && n.IDLoaiPhong == LoaiPhong && n.IDKhuVuc == KhuVuc && n.TenPhong.ToLower().Contains(tenphong));
                                return View(phong.ToPagedList(pageNum, pageSize));
                            }
                        }
                    }
                }
            }
        }

        public ActionResult ThemPhong()
        {
            ViewBag.IDKieuPhong = new System.Web.Mvc.SelectList((from p in data.KieuPhongs
                                                                 select p).ToList(), "IDKieuPhong", "TenKieuPhong");
            ViewBag.IDKhuVuc = new System.Web.Mvc.SelectList((from p in data.KhuVucs
                                                              select p).ToList(), "IDKhuVuc", "TenKV");
            ViewBag.IDLoaiPhong = new System.Web.Mvc.SelectList((from p in data.LoaiPhongs
                                                                 select p).ToList(), "IDLoaiPhong", "TenLoaiPhong");
            return View();
        }
        [HttpPost]
        public ActionResult ThemPhong(FormCollection collection, Phong phong, int? IDKieuPhong, int? IDKhuVuc, int? IDLoaiPhong)
        {
            var TenPhong = collection["TenPhong"];
            var GiaPhong = Convert.ToDouble(collection["GiaPhong"]);
            var AnhDaiDien = collection["AnhDaiDien"];
            var MoTa = collection["MoTa"];
            var ViTri = collection["Vitri"];

            if (string.IsNullOrEmpty(TenPhong))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                phong.TenPhong = TenPhong;
                phong.GiaPhong = GiaPhong;
                phong.IDKhuVuc = IDKhuVuc;
                phong.AnhDaiDien = AnhDaiDien;
                phong.MoTa = MoTa;
                phong.Vitri = ViTri;
                phong.TrangThai = true;
                phong.IDKieuPhong = IDKieuPhong;
                data.Phongs.InsertOnSubmit(phong);
                data.SubmitChanges();
                return RedirectToAction("Phong");
            }
            return this.ThemPhong();
        }
        public ActionResult SuaPhong(int id)
        {
            var phong = data.Phongs.First(n => n.MaPhong == id);
            if (phong == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }

            ViewBag.IDKieuPhong = new System.Web.Mvc.SelectList((from p in data.KieuPhongs
                                                                 select p).ToList(), "IDKieuPhong", "TenKieuPhong", phong.IDKieuPhong);

            ViewBag.IDKhuVuc = new System.Web.Mvc.SelectList((from p in data.KhuVucs
                                                              select p).ToList(), "IDKhuVuc", "TenKV", phong.IDKhuVuc);
            ViewBag.IDLoaiPhong = new System.Web.Mvc.SelectList((from p in data.LoaiPhongs
                                                                 select p).ToList(), "IDLoaiPhong", "TenLoaiPhong", phong.IDLoaiPhong);
            return View(phong);
        }
        [HttpPost]
        public ActionResult SuaPhong(int id, FormCollection collection, int? IDKieuPhong, int? IDKhuVuc, int? IDLoaiPhong)
        {
            var phong = data.Phongs.First(n => n.MaPhong == id);
            var TenPhong = collection["TenPhong"];
            var GiaPhong = Convert.ToDouble(collection["GiaPhong"]);
            var AnhDaiDien = collection["AnhDaiDien"];
            var MoTa = collection["MoTa"];
            var ViTri = collection["Vitri"];

            if (String.IsNullOrEmpty(TenPhong))
            {
                ViewData["Loi1"] = "Tên loại sản phẩm không được để trống";
            }
            else
            {
                phong.TenPhong = TenPhong;
                phong.GiaPhong = GiaPhong;
                phong.IDKhuVuc = IDKhuVuc;
                phong.AnhDaiDien = AnhDaiDien;
                phong.MoTa = MoTa;
                phong.Vitri = ViTri;
                phong.TrangThai = true;
                phong.IDKieuPhong = IDKieuPhong;
                UpdateModel(phong);
                data.SubmitChanges();
                return RedirectToAction("Phong");
            }
            return this.SuaPhong(id);
        }

        public ActionResult XoaPhong(int id)
        {
            try
            {
                Phong Phong = data.Phongs.SingleOrDefault(n => n.MaPhong == id);
                if (Phong == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.Phongs.DeleteOnSubmit(Phong);
                data.SubmitChanges();
                return Json(new
                {
                    result = true
                        ,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = true
                        ,
                    Message = "Lỗi"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Tài khoản - Hóa đơn
        public ActionResult HoaDon(int? page, int? HoaDon)
        {
            try
            {
                if (page == null) page = 1;
                int pageSize = 20;
                int pageNum = page ?? 1;
                ViewBag.Keyword = HoaDon;
                if (HoaDon == null)
                {
                    var khuVucs = data.HoaDons;
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
                else
                {
                    var khuVucs = data.HoaDons.Where(n => n.IDHoaDon == HoaDon);
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
            }
            catch
            {
                return RedirectToAction("HoaDon");
            }
        }

        public ActionResult ChiTietHoaDon(int? page, int? HoaDon, int? TaiKhoan)
        {
            try
            {
                if (page == null) page = 1;
                int pageSize = 2;
                int pageNum = page ?? 1;
                ViewBag.Keyword = HoaDon;
                if (HoaDon == null)
                {
                    var khuVucs = data.HoaDons.Where(n => n.IDTaiKhoan == TaiKhoan);
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
                else
                {
                    var khuVucs = data.HoaDons.Where(n => n.IDHoaDon == HoaDon && n.IDTaiKhoan == TaiKhoan);
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
            }
            catch
            {
                return RedirectToAction("HoaDon");
            }
        }

        public ActionResult TaiKhoan(int? page, string timkiem)
        {
            if (page == null) page = 1;
            int pageSize = 2;
            int pageNum = page ?? 1;
            ViewBag.Keyword = timkiem;
            if (String.IsNullOrEmpty(timkiem))
            {
                var khuVucs = data.TaiKhoans;
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var khuVucs = data.TaiKhoans.Where(n => n.HoTen.ToLower().Contains(timkiem) || n.Email.ToLower().Contains(timkiem));
                return View(khuVucs.ToPagedList(pageNum, pageSize));
            }
        }

        public ActionResult ChiTietTaiKhoan(int? TaiKhoan)
        {
            var khuVuc = data.TaiKhoans.First(n => n.IDTaiKhoan == TaiKhoan);
            if (TaiKhoan == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            return View(khuVuc);
        }


        public ActionResult SuaTaiKhoan(int id)
        {
            var khuVuc = data.KhuVucs.First(n => n.IDKhuVuc == id);
            if (khuVuc == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            return View(khuVuc);
        }
        [HttpPost]
        public ActionResult SuaTaiKhoan(int id, FormCollection collection)
        {
            var khuVuc = data.LoaiPhongs.First(n => n.IDLoaiPhong == id);
            var TenLoaiPhong = collection["TenLoaiPhong"];
            var SoLuongPhong = Convert.ToInt32(collection["SoLuongPhong"]);
            var AnhDaiDien = collection["AnhDaiDien"];

            if (String.IsNullOrEmpty(TenLoaiPhong))
            {
                ViewData["Loi1"] = "Tên loại sản phẩm không được để trống";
            }
            else
            {
                khuVuc.TenLoaiPhong = TenLoaiPhong;
                khuVuc.SoLuongPhong = SoLuongPhong;
                khuVuc.AnhDaiDien = AnhDaiDien;
                UpdateModel(khuVuc);
                data.SubmitChanges();
                return RedirectToAction("LoaiPhong");
            }
            return this.SuaTaiKhoan(id);
        }

        public ActionResult XoaTaiKhoan(int id)
        {
            try
            {
                LoaiPhong loaiPhong = data.LoaiPhongs.SingleOrDefault(n => n.IDLoaiPhong == id);
                if (loaiPhong == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.LoaiPhongs.DeleteOnSubmit(loaiPhong);
                data.SubmitChanges();
                return RedirectToAction("LoaiPhong");
            }
            catch
            {
                return RedirectToAction("LoaiPhong");
            }
        }

        public ActionResult DetailsHD(int id)
        {
            var ct = data.HoaDons.Where(p => p.IDHoaDon == id);
            return PartialView("DetailsHD", ct);
        }

        #endregion

        #region Ảnh phòng
        public ActionResult AnhPhong(int? page, string timkiem, int? maPhong)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNum = page ?? 1;
            ViewBag.maPhong = new System.Web.Mvc.SelectList((from p in data.Phongs
                                                             select p).ToList(), "MaPhong", "TenPhong");
            ViewBag.Keyword = timkiem;
            if (String.IsNullOrEmpty(timkiem))
            {
                if (maPhong == null)
                {
                    var khuVucs = data.AnhPhongs;
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
                else
                {
                    var khuVucs = data.AnhPhongs.Where(n => n.MaPhong == maPhong);
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
            }
            else
            {
                if (maPhong == null)
                {
                    var khuVucs = data.AnhPhongs.Where(n => n.Phong.TenPhong.ToLower().Contains(timkiem));
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
                else
                {
                    var khuVucs = data.AnhPhongs.Where(n => n.MaPhong == maPhong && n.Phong.TenPhong.ToLower().Contains(timkiem));
                    return View(khuVucs.ToPagedList(pageNum, pageSize));
                }
            }
        }

        public ActionResult ThemAnhPhong()
        {
            ViewBag.maPhong = new System.Web.Mvc.SelectList((from p in data.Phongs
                                                             select p).ToList(), "MaPhong", "TenPhong");
            return View();
        }
        [HttpPost]
        public ActionResult ThemAnhPhong(FormCollection collection, AnhPhong anhPhong, int? maPhong)
        {
            try
            {
                var Url = collection["Url"];
                var TinhTrang = collection["TinhTrang"];

                anhPhong.TinhTrang = TinhTrang;
                anhPhong.Url = Url;
                anhPhong.MaPhong = maPhong;
                data.AnhPhongs.InsertOnSubmit(anhPhong);
                data.SubmitChanges();
                return RedirectToAction("ThemAnhPhong");
            }
            catch
            {
                return RedirectToAction("ThemAnhPhong");
            }
        }
        public ActionResult SuaAnhPhong(int id)
        {

            var khuVuc = data.AnhPhongs.First(n => n.IDAnhPhong == id);
            if (khuVuc == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            ViewBag.maPhong = new System.Web.Mvc.SelectList((from p in data.Phongs
                                                             select p).ToList(), "MaPhong", "TenPhong");
            return View(khuVuc);
        }
        [HttpPost]
        public ActionResult SuaAnhPhong(int id, FormCollection collection)
        {
            try
            {
                var anhPhong = data.AnhPhongs.First(n => n.IDAnhPhong == id);
                var Url = collection["Url"];
                var TinhTrang = collection["TinhTrang"];

                anhPhong.TinhTrang = TinhTrang;
                anhPhong.Url = Url;
                UpdateModel(anhPhong);
                data.SubmitChanges();
                return RedirectToAction("AnhPhong");
            }
            catch
            {
                return RedirectToAction("AnhPhong");
            }

        }

        public ActionResult XoaAnhPhong(int id)
        {
            try
            {
                AnhPhong anhPhong = data.AnhPhongs.SingleOrDefault(n => n.IDAnhPhong == id);
                if (anhPhong == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.AnhPhongs.DeleteOnSubmit(anhPhong);
                data.SubmitChanges();
                return Json(new
                {
                    result = true
                        ,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = false
                        ,
                    Message = "Lỗi"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Chi tiết tiện nghi

        public ActionResult ChiTietTienNghi(int? page, string timkiem, int? maPhong, int? tienNghi)
        {
            if (page == null) page = 1;
            int pageSize = 20;
            int pageNum = page ?? 1;
            ViewBag.Keyword = timkiem;
            ViewBag.maPhong = new SelectList((data.Phongs).ToList(), "MaPhong", "TenPhong");
            ViewBag.tienNghi = new SelectList((data.TienNghis).ToList(), "IDTN", "TenTN");
            if (String.IsNullOrEmpty(timkiem))
            {
                if (maPhong == null)
                {
                    if (tienNghi == null)
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong);
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                    else
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong).Where(n => n.IDTN == tienNghi);
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                }
                else
                {
                    if (tienNghi == null)
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong).Where(n => n.MaPhong == maPhong);
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                    else
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong).Where(n => n.IDTN == tienNghi && n.MaPhong == maPhong);
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                }
            }
            else
            {
                if (maPhong == null)
                {
                    if (tienNghi == null)
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong).Where(n => n.Phong.TenPhong.ToLower().Contains(timkiem));
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                    else
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong).Where(n => n.IDTN == tienNghi && n.Phong.TenPhong.ToLower().Contains(timkiem));
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                }
                else
                {
                    if (tienNghi == null)
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong).Where(n => n.MaPhong == maPhong && n.Phong.TenPhong.ToLower().Contains(timkiem));
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                    else
                    {
                        var khuVucs = data.ChiTietTienNghis.OrderBy(m => m.MaPhong).Where(n => n.IDTN == tienNghi && n.MaPhong == maPhong && n.Phong.TenPhong.ToLower().Contains(timkiem));
                        return View(khuVucs.ToPagedList(pageNum, pageSize));
                    }
                }
            }
        }

        public ActionResult ThemChiTietTienNghi()
        {
            ViewBag.maPhong = new SelectList((data.Phongs).ToList(), "MaPhong", "TenPhong");
            ViewBag.tienNghi = new SelectList((data.TienNghis).ToList(), "IDTN", "TenTN");
            return View();
        }
        [HttpPost]
        public ActionResult ThemChiTietTienNghi(FormCollection collection, ChiTietTienNghi chiTietTienNghi, int maPhong, int tienNghi)
        {
            try
            {
                var SoLuong = Convert.ToInt32(collection["Soluong"]);
                chiTietTienNghi = new ChiTietTienNghi();
                chiTietTienNghi.MaPhong = maPhong;
                chiTietTienNghi.IDTN = tienNghi;
                chiTietTienNghi.Soluong = SoLuong;
                chiTietTienNghi.TinhTrang = true;
                data.ChiTietTienNghis.InsertOnSubmit(chiTietTienNghi);
                data.SubmitChanges();
                return RedirectToAction("ChiTietTienNghi");
            }
            catch
            {
                return RedirectToAction("ThemChiTietTienNghi");
            }

        }
        public ActionResult SuaChiTietTienNghi(int maPhong, int IDTN)
        {
            var tienNghi = data.ChiTietTienNghis.First(n => n.IDTN == IDTN && n.MaPhong == maPhong);
            if (tienNghi == null)
            {
                Response.SubStatusCode = 404;
                return null;
            }
            ViewBag.mp = new SelectList((data.Phongs).ToList(), "MaPhong", "TenPhong",tienNghi.MaPhong);
            ViewBag.idtiennghi = new SelectList((data.TienNghis).ToList(), "IDTN", "TenTN",tienNghi.IDTN);
            return View(tienNghi);
        }
        [HttpPost]
        public ActionResult SuaChiTietTienNghi(int maPhong, int IDTN, FormCollection collection, int mp, int idtiennghi)
        {
            var chiTietTienNghi = data.ChiTietTienNghis.First(n => n.MaPhong == maPhong && n.IDTN == IDTN);
            try
            {
                var SoLuong = Convert.ToInt32(collection["Soluong"]);
                chiTietTienNghi.MaPhong = maPhong;
                chiTietTienNghi.IDTN = idtiennghi;
                chiTietTienNghi.TinhTrang = true;
                chiTietTienNghi.Soluong = SoLuong;
                UpdateModel(chiTietTienNghi);
                data.SubmitChanges();
                return RedirectToAction("ChiTietTienNghi");
            }
            catch
            {
                return this.SuaChiTietTienNghi(mp, idtiennghi);
            }
        }

        public ActionResult XoaChiTietTienNghi(int maPhong, int IDTN)
        {
            try
            {
                ChiTietTienNghi kieuPhong = data.ChiTietTienNghis.SingleOrDefault(n => n.IDTN == IDTN && n.MaPhong == maPhong);
                if (kieuPhong == null)
                {
                    Response.SubStatusCode = 404;
                    return null;
                }
                data.ChiTietTienNghis.DeleteOnSubmit(kieuPhong);
                data.SubmitChanges();
                return Json(new
                {
                    result = true
                        ,
                    Message = "Thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    result = false
                        ,
                    Message = "Lỗi"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Lung tung
        public JsonResult StatusPhong(int IDSP)
        {
            var result = ChangePhong(IDSP);
            return Json(new
            {
                message = "Thay Đổi Trạng Thái Thành Công",
                abc = result,

            }, JsonRequestBehavior.AllowGet); ;
        }
        public JsonResult StatusLoaiPhong(int IDSP)
        {
            var result = ChangeLoaiPhong(IDSP);
            return Json(new
            {
                message = "Thay Đổi Trạng Thái Thành Công",
                abc = result,

            }, JsonRequestBehavior.AllowGet); ;
        }

        public JsonResult StatusTaiKhoan(int IDSP)
        {
            var result = ChangeTaiKhoan(IDSP);
            return Json(new
            {
                message = "Thay Đổi Quyền Thành Công",
                abc = result,

            }, JsonRequestBehavior.AllowGet); ;
        }

        public bool? ChangeTaiKhoan(int IDSP)
        {
            var a = data.TaiKhoans.FirstOrDefault(c => c.IDTaiKhoan == IDSP);
            a.Quyen = !a.Quyen;
            data.SubmitChanges();
            return a.Quyen;
        }

        public bool? ChangePhong(int IDSP)
        {
            var a = data.Phongs.FirstOrDefault(c => c.MaPhong == IDSP);
            a.TrangThai = !a.TrangThai;
            data.SubmitChanges();
            return a.TrangThai;
        }

        public bool? ChangeLoaiPhong(int IDSP)
        {
            var a = data.LoaiPhongs.FirstOrDefault(c => c.IDLoaiPhong == IDSP);
            a.TrangThai = !a.TrangThai;
            data.SubmitChanges();
            return a.TrangThai;
        }

        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }

        #endregion
    }
}
