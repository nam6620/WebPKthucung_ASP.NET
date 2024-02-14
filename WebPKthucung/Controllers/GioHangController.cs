using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using WebPKthucung.Models;
namespace WebPKthucung.Controllers
{
    public class GioHangController : Controller
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
        string LayMa()
        {
            var maMax = db.DONDATHANGs.ToList().Select(n => n.MADH).Max();
            int maAD;
            if (maMax == null)
            {
                maAD = 1;
            }
            else
            {
                maAD = int.Parse(maMax.Substring(2)) + 1;
            }
            string AD = String.Concat("00", maAD.ToString());
            return "DH" + AD.Substring(maAD.ToString().Length - 1);
        }
        string layDiaChi(string maXa, string tenDuong)
        {
            XA xa = db.XAs.FirstOrDefault(n => n.MAXA == maXa);
            HUYEN huyen = db.HUYENs.FirstOrDefault(n => n.MAHUYEN == xa.MAHUYEN);
            TINH tinh = db.TINHs.FirstOrDefault(n => n.MATINH == huyen.MATINH);
            string diaChi = tenDuong + "/" + xa.TENTINH + "/" + huyen.TENTINH + "/" + tinh.TENTINH;
            //Console.WriteLine("maXa = {0}, tenDuong = {1}, xa.TENTINH = {2}, huyen.TENTINH = {3}, tinh.TENTINH = {4}, diaChi = {5}", maXa, tenDuong, xa.TENTINH, huyen.TENTINH, tinh.TENTINH, diaChi);
            return diaChi;
        }
        public List<GioHang> Laygiohang()
        {
            List<GioHang> dsGiohang = Session["Giohang"] as List<GioHang>;
            if (dsGiohang == null)
            {
                dsGiohang = new List<GioHang>();
                Session["Giohang"] = dsGiohang;
            }
            return dsGiohang;
        }

        public ActionResult ThemGiohang(String iMASP, string strURL)
        {
            List<GioHang> dsGiohang = Laygiohang();
            GioHang sanpham = dsGiohang.Find(n => n.iMASP == iMASP);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMASP);
                dsGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSOLUONG++;
                return Redirect(strURL);
            }
        }
        public ActionResult botSanPhamGiohang(String iMASP, string strURL)
        {
            List<GioHang> dsGiohang = Laygiohang();
            GioHang sanpham = dsGiohang.Find(n => n.iMASP == iMASP);
            if(sanpham.iSOLUONG == 1)
            {
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSOLUONG--;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> dsGiohang = Session["GioHang"] as List<GioHang>;
            if (dsGiohang != null)
            {
                iTongSoLuong = dsGiohang.Sum(n => n.iSOLUONG);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> dsGiohang = Session["GioHang"] as List<GioHang>;
            if (dsGiohang != null)
            {
                iTongTien = dsGiohang.Sum(n => n.dTHANHTIEN);
            }
            return iTongTien;
        }
        private double TongTienSauGiamGia()
        {
            PHIEUGIAMGIA phieugiamgia = Session["MaPhieuGiamGia"] as PHIEUGIAMGIA;
            if (phieugiamgia != null)
            {
                if (phieugiamgia.LOAIGIAMGIA == true)
                {
                    return TongTien() * (100 - phieugiamgia.GIATRIGIAM) / 100;
                }
                else
                if (phieugiamgia.LOAIGIAMGIA == false)
                {
                    return TongTien() - phieugiamgia.GIATRIGIAM;
                }
            }
            return TongTien();
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.TongTienSauGiamGia = TongTienSauGiamGia();
            return PartialView();
        }
        public ActionResult GioHang()
        {
            List<GioHang> dsGiohang = Laygiohang();
            ViewBag.Tongtien = TongTien();
            ViewBag.TongTienSauGiamGia = TongTienSauGiamGia();
            return View(dsGiohang);
        }
        [HttpGet]
        public ActionResult PhieuGiamGia()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult PhieuGiamGia(string code)
        {
            PHIEUGIAMGIA phieugiamgia = db.PHIEUGIAMGIAs.SingleOrDefault(n => n.CODE == code && n.NGAYBATDAU <= DateTime.Now && n.NGAYKETTHUC >= DateTime.Now && n.SOLANSUDUNG>0);
            if (phieugiamgia ==  null)
            {
                Session["PhieuGiamGia"] = "Phiếu giảm giá không hợp lệ";
                
            }
            else
            {
                Session["PhieuGiamGia"] = "Áp mã thành công!";
                Session["MaPhieuGiamGia"] = phieugiamgia;
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaGiohang(String iMaSP)
        {
            List<GioHang> dsGiohang = Laygiohang();
            GioHang sanpham = dsGiohang.SingleOrDefault(n => n.iMASP == iMaSP);
            if (sanpham != null)
            {
                dsGiohang.RemoveAll(n => n.iMASP == iMaSP);
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            List<GioHang> dsGiohang = Laygiohang();
            dsGiohang.Clear();
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("dangnhap", "Users");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("sanpham", "Users");
            }
            //Lay gio hang tu Session
            List<GioHang> lstGiohang = Laygiohang();
            KHANHHANG kh = (KHANHHANG)Session["Taikhoan"];
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTienSauGiamGia();
            ViewBag.DiaChi = layDiaChi(kh.MAXA, kh.DAICHI);
            return View(lstGiohang);
        }

        #region Thêm đơn đặt hàng mới
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHANHHANG kh = (KHANHHANG)Session["Taikhoan"];
            List<GioHang> gh = Laygiohang();
            ddh.MADH = LayMa();
            ddh.MAKH = kh.MAKH;
            ddh.NGAYDAT = DateTime.Now;
            ddh.TINHTRANGDH = false;
            ddh.DATHANHTOAN = false;
            ddh.TONGTIEN = (decimal)TongTien();
            PHIEUGIAMGIA p = (PHIEUGIAMGIA)Session["MaPhieuGiamGia"];
            if (p != null)
            {
                ddh.PHIEUGIAMGIA = p.CODE;
                p.SOLANSUDUNG--;
                UpdateModel(p);
                db.SaveChanges();
                Session["MaPhieuGiamGia"] = null;
                Session["PhieuGiamGia"] = null;
            } 
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();
            foreach (var item in gh)
            {
                SANPHAM sanpham = db.SANPHAMs.Single(n => n.MASP == item.iMASP);
                if (sanpham.SOLUONG >= item.iSOLUONG)
                {
                    CTDONDATHANG ctdh = new CTDONDATHANG();
                    ctdh.MADH = ddh.MADH;
                    ctdh.MASP = item.iMASP;
                    ctdh.SOLUONG = item.iSOLUONG;
                    ctdh.DONGIA = (int)item.dDONGIA;
                    ctdh.THANHTIEN = (decimal)item.dTHANHTIEN;
                    db.CTDONDATHANGs.Add(ctdh);
                    sanpham.SOLUONG = sanpham.SOLUONG - item.iSOLUONG;
                    db.SaveChanges();
                    Session["Giohang"] = null;
                }
                else
                {
                    return RedirectToAction("ThongBao", "Giohang");
                }

            }
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        #endregion



        public ActionResult ThongBao()
        {
            return View();
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }

        #region Lấy hình thương hiệu
        public ActionResult hinhthuonghieu()
        {
            var listthuonghieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;

            return PartialView(listthuonghieu);
        }
        #endregion

    }
}