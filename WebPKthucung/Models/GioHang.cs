using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPKthucung.Models
{
    public class GioHang
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        /*FormCollection f;*/
        public String iMASP { set; get; }
        public string gTENSP { set; get; }
        public string gHINHANH { set; get; }
        public string gTHUONGHIEU { set; get; }
        public Double dDONGIA { set; get; }
        public int iSOLUONG { set; get; }
        public int iGIAMGIA { set; get; }
        public Double dTHANHTIEN
        {
            get { return iSOLUONG * dDONGIA * (100 - iGIAMGIA)/100; }

        }
        public Double iTONGTIEN { set; get; }

        public GioHang(String MASP)
        {

            iMASP = MASP;
            SANPHAM sanpham = db.SANPHAMs.Single(n => n.MASP == iMASP);
            gTENSP = sanpham.TENSP;
            gHINHANH = sanpham.HINHANH;
            dDONGIA = double.Parse(sanpham.DONGIABAN.ToString());
            iSOLUONG = 1;
            GIAMGIA giamgia = db.GIAMGIAs.SingleOrDefault(n => n.MASP == iMASP && n.NGAYBATDAU <= DateTime.Now && n.NGAYKETTHUC >= DateTime.Now);
            if (giamgia == null)
            {
                iGIAMGIA = 0;
            }   
            else
            {
                iGIAMGIA = giamgia.PHAMTRAMGIAM;
            }
            gTHUONGHIEU = sanpham.THUONGHIEU.TENTH;
        }
    }
}