using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHotelThienAn.Models
{
    public class HoaHon
    {

        MyDataContextDataContext data = new MyDataContextDataContext();
        public int MaPhong { get; set; }

        [Display(Name = "Tên Phòng")]
        public string TenPhong { get; set; }


        [Display(Name = "Đơn giá Phòng")]
        public double GiaPhong { get; set; }


        [Display(Name = "Ảnh Đại Diện")]
        public string ANHBIA { get; set; }


        public HoaHon(int id)
        {
            MaPhong = id;
            Phong sp = data.Phongs.Single(m => m.MaPhong == MaPhong);
            TenPhong = sp.TenPhong;
            GiaPhong = double.Parse(sp.GiaPhong.ToString());
            ANHBIA = sp.AnhDaiDien;
        }

    }
}