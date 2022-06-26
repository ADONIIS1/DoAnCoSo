using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHotelThienAn.Models
{
    public class Phongdat
    {
        public Phong phong { get; set; }

        public int SoNgay { get; set; }
        public int MucThanhToan { get; set; }
        
        public double ThanhTien => (double)(phong.GiaPhong * SoNgay);
    }
}