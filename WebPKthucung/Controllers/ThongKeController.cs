using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using WebPKthucung.App_Start;
using WebPKthucung.Models;


namespace WebPKthucung.Controllers
{
    public class ThongKeController : Controller
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: ThongKe
        [HttpGet]
        public ActionResult Index()
        {
            var thongKe = new List<BaoCao>();
            int pageSize = 20;
            var list = thongKe.OrderBy(x => x.NgayBan);
            ViewBag.tongTienTheoThangNam = new List<BaoCaoThang>();
            ViewBag.TongDoanhThu = 0;
            return View(list.ToPagedList(1, pageSize));
        }
        [HttpPost]
        public ActionResult Index(int? page, int thangbatdau, int thangketthuc, int nambatdau, int namketthuc)
        {
            
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            } else
            {
                ViewBag.thangbatdau = thangbatdau;
                ViewBag.thangketthuc = thangketthuc;
                ViewBag.nambatdau = nambatdau;
                ViewBag.namketthuc = namketthuc;
                DateTime batDau = new DateTime(nambatdau, thangbatdau, 1);
                DateTime ketThuc = new DateTime(namketthuc, thangketthuc, 30);
                var thongKe = (from a in db.DONDATHANGs where  a.NGAYDAT >= batDau &&  a.NGAYDAT <=ketThuc
                               join b in db.CTDONDATHANGs on a.MADH equals b.MADH
                               join c in db.SANPHAMs on b.MASP equals c.MASP
                               select new BaoCao
                               {
                                   MADH = a.MADH,
                                   maSanPham = b.MASP,
                                   TenSanPham = c.TENSP,
                                   SoLuongBan = b.SOLUONG,
                                   DonGiaBan = b.DONGIA,
                                   thanhTien = b.THANHTIEN,
                                   NgayBan = a.NGAYDAT,
                               }).OrderBy(x => x.NgayBan).ToList();
                var tongTienTheoThangNam = db.DONDATHANGs
                                            .Where(d =>  d.NGAYDAT >= batDau && d.NGAYDAT <= ketThuc)
                                            .GroupBy(d => new { d.NGAYDAT.Year, d.NGAYDAT.Month })
                                            .Select(g => new { Thang = g.Key.Month, Nam = g.Key.Year, TongTien = g.Sum(d => d.TONGTIEN) })
                                            .OrderBy(g => g.Nam)
                                            .ThenBy(g => g.Thang)
                                            .ToList();
                ViewBag.tongTienTheoThangNam = tongTienTheoThangNam.Select(d => new BaoCaoThang
                {
                    ThangBan = new DateTime(d.Nam, d.Thang, 1),
                    TongTien = d.TongTien
                }).ToList();
                ViewBag.TongDoanhThu = tongTienTheoThangNam.Sum(item => item.TongTien);
                ViewBag.baoCao = thongKe;
                int pageSize = 20;
                var pageNumber = page ?? 1;
                var list = thongKe.OrderBy(x => x.NgayBan);
                if (list.Count() == 0) ViewBag.TB = "Không có sản phẩm nào được bán trong thời gian này";
                return View(list.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult InBaoCao(int thangbatdau, int thangketthuc, int nambatdau, int namketthuc)
        {
            ViewBag.thangbatdau = thangbatdau;
            ViewBag.thangketthuc = thangketthuc;
            ViewBag.nambatdau = nambatdau;
            ViewBag.namketthuc = namketthuc;
            DateTime batDau = new DateTime(nambatdau, thangbatdau, 1);
            DateTime ketThuc = new DateTime(namketthuc, thangketthuc, 30);
            var thongKe = (from a in db.DONDATHANGs
                           where  a.NGAYDAT >= batDau && a.NGAYDAT <= ketThuc
                           join b in db.CTDONDATHANGs on a.MADH equals b.MADH
                           join c in db.SANPHAMs on b.MASP equals c.MASP
                           select new BaoCao
                           {
                               MADH = a.MADH,
                               maSanPham = b.MASP,
                               TenSanPham = c.TENSP,
                               SoLuongBan = b.SOLUONG,
                               DonGiaBan = b.DONGIA,
                               thanhTien = b.THANHTIEN,
                               NgayBan = a.NGAYDAT,
                           }).OrderBy(x => x.NgayBan).ToList();
            var tongTienTheoThangNam = db.DONDATHANGs
                                        .Where(d =>  d.NGAYDAT >= batDau && d.NGAYDAT <= ketThuc)
                                        .GroupBy(d => new { d.NGAYDAT.Year, d.NGAYDAT.Month })
                                        .Select(g => new { Thang = g.Key.Month, Nam = g.Key.Year, TongTien = g.Sum(d => d.TONGTIEN) })
                                        .OrderBy(g => g.Nam)
                                        .ThenBy(g => g.Thang)
                                        .ToList();
            ViewBag.tongTienTheoThangNam = tongTienTheoThangNam.Select(d => new BaoCaoThang
            {
                ThangBan = new DateTime(d.Nam, d.Thang, 1),
                TongTien = d.TongTien
            }).ToList();
            ViewBag.TongDoanhThu = tongTienTheoThangNam.Sum(item => item.TongTien);
            ViewBag.baoCao = thongKe;
            var list = thongKe.OrderBy(x => x.NgayBan);
            return View(list);
        }
    }
}