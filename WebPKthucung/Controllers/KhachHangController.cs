using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using WebPKthucung.App_Start;
using WebPKthucung.Models;

namespace WebPKthucung.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_KHACHHANG")]
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        string layDiaChi(string maXa, string tenDuong)
        {
            XA xa = db.XAs.FirstOrDefault(n => n.MAXA == maXa);
            HUYEN huyen = db.HUYENs.FirstOrDefault(n => n.MAHUYEN == xa.MAHUYEN);
            TINH tinh = db.TINHs.FirstOrDefault(n => n.MATINH == huyen.MATINH);
            string diaChi = tenDuong + "/" + xa.TENTINH + "/" + huyen.TENTINH + "/" + tinh.TENTINH;
            //Console.WriteLine("maXa = {0}, tenDuong = {1}, xa.TENTINH = {2}, huyen.TENTINH = {3}, tinh.TENTINH = {4}, diaChi = {5}", maXa, tenDuong, xa.TENTINH, huyen.TENTINH, tinh.TENTINH, diaChi);
            return diaChi;
        }
        public int TinhTuoi(DateTime ngaySinh)
        {
            DateTime ngayHienTai = DateTime.Today;
            int tuoi = ngayHienTai.Year - ngaySinh.Year;
            if (ngayHienTai.Month < ngaySinh.Month || (ngayHienTai.Month == ngaySinh.Month && ngayHienTai.Day < ngaySinh.Day))
            {
                tuoi--;
            }
            return tuoi;
        }
        public ActionResult Index(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                if (page == null) page = 1;
                int pageSize = 7;
                var pageNumber = page ?? 1;
                var kh = from KHANHHANG in db.KHANHHANGs select KHANHHANG;
                var list = kh.OrderBy(x => x.MAKH);
                return View(list.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Search(int? page, string maKH = "", string tenKH = "", string dienthoai = "", string email = "", int tuoiBatDau = 0, int tuoiKetThuc =100, string diachi= "", string ngaysinh = "")
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                ViewBag.maKH = maKH;
                ViewBag.tenKH = tenKH;
                ViewBag.dienthoai = dienthoai;
                ViewBag.email = email;
                ViewBag.tuoiBT = tuoiBatDau;
                ViewBag.ngaysinh = ngaysinh;
                ViewBag.diachi = diachi;
                ViewBag.tuoiKT = tuoiKetThuc;
                var allacc = (from a in db.KHANHHANGs
                              select new Customer
                              {
                                  MAKH = a.MAKH,
                                  DIENTHOAI = a.DIENTHOAI,
                                  DAICHI = a.DAICHI,
                                  HOTENKH = a.HOTENKH,
                                  EMAIL = a.EMAIL,
                                  NGAYSINH = a.NGAYSINH,
                                  HINHANH = a.HINHANH,
                                  MAXA = a.MAXA,
                              }).OrderBy(x => x.MAKH).ToList();
                foreach (var kh in allacc)
                {
                    kh.iDIACHI = layDiaChi(kh.MAXA,kh.DAICHI);
                    kh.TUOI = TinhTuoi(kh.NGAYSINH);
                }
                if (!string.IsNullOrEmpty(maKH))
                {
                    allacc = allacc.Where(c => c.MAKH.Contains(maKH)).ToList();
                }
                if (!string.IsNullOrEmpty(tenKH))
                {
                    allacc = allacc.Where(s => s.HOTENKH.Contains(tenKH)).ToList();
                }
                if (!string.IsNullOrEmpty(dienthoai))
                {
                    allacc = allacc.Where(s => s.DIENTHOAI.Contains(dienthoai)).ToList();
                }
                if (!string.IsNullOrEmpty(email))
                {
                    allacc = allacc.Where(s => s.EMAIL.Contains(email)).ToList();
                }
                if (!string.IsNullOrEmpty(diachi))
                {
                    allacc = allacc.Where(s => s.iDIACHI.Contains(diachi)).ToList();
                }
                if (!string.IsNullOrEmpty(ngaysinh))
                {
                    DateTime dateOfBirth = DateTime.ParseExact(ngaysinh, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    allacc = allacc.Where(s => s.NGAYSINH.Date == dateOfBirth).ToList();
                }
                allacc = allacc.Where(s => s.TUOI > tuoiBatDau && s.TUOI < tuoiKetThuc).ToList();
                if (allacc.Count() == 0)
                {
                    ViewBag.TB = "Không tìm thấy dữ liệu";
                }
                else
                {
                    ViewBag.TB = null;
                }
                if (page == null) page = 1;
                int pageSize = 7;
                var pageNumber = page ?? 1;
                var list = allacc.OrderBy(x => x.MAKH);
                return View(list.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult InBaoCao(string maKH = "", string tenKH = "", string dienthoai = "", string email = "", int tuoiBatDau = 0, int tuoiKetThuc = 100, string diachi = "", string ngaysinh = "")
        {
            var allacc = (from a in db.KHANHHANGs
                          select new Customer
                          {
                              MAKH = a.MAKH,
                              DIENTHOAI = a.DIENTHOAI,
                              DAICHI = a.DAICHI,
                              HOTENKH = a.HOTENKH,
                              EMAIL = a.EMAIL,
                              NGAYSINH = a.NGAYSINH,
                              HINHANH = a.HINHANH,
                              MAXA = a.MAXA,
                          }).OrderBy(x => x.MAKH).ToList();
            foreach (var kh in allacc)
            {
                kh.iDIACHI = layDiaChi(kh.MAXA, kh.DAICHI);
                kh.TUOI = TinhTuoi(kh.NGAYSINH);
            }
            if (!string.IsNullOrEmpty(maKH))
            {
                allacc = allacc.Where(c => c.MAKH.Contains(maKH)).ToList();
            }
            if (!string.IsNullOrEmpty(tenKH))
            {
                allacc = allacc.Where(s => s.HOTENKH.Contains(tenKH)).ToList();
            }
            if (!string.IsNullOrEmpty(dienthoai))
            {
                allacc = allacc.Where(s => s.DIENTHOAI.Contains(dienthoai)).ToList();
            }
            if (!string.IsNullOrEmpty(email))
            {
                allacc = allacc.Where(s => s.EMAIL.Contains(email)).ToList();
            }
            if (!string.IsNullOrEmpty(diachi))
            {
                allacc = allacc.Where(s => s.iDIACHI.Contains(diachi)).ToList();
            }
            if (!string.IsNullOrEmpty(ngaysinh))
            {
                DateTime dateOfBirth = DateTime.ParseExact(ngaysinh, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                allacc = allacc.Where(s => s.NGAYSINH.Date == dateOfBirth).ToList();
            }
            allacc = allacc.Where(s => s.TUOI > tuoiBatDau && s.TUOI < tuoiKetThuc).ToList();
            if (allacc.Count() == 0)
            {
                ViewBag.TB = "Không tìm thấy dữ liệu";
            }
            else
            {
                ViewBag.TB = null;
            }
           
            var list = allacc.OrderBy(x => x.MAKH);
            return View(list);
        }
        public ActionResult Detail(string id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var khachhang = from kh in db.KHANHHANGs where kh.MAKH == id select kh;
                KHANHHANG k = khachhang.FirstOrDefault();
                ViewBag.diachi = layDiaChi(k.MAXA, k.DAICHI);
                return View(khachhang.Single());
            }
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var khachhang = from kh in db.KHANHHANGs where kh.MAKH == id select kh;
                KHANHHANG k = khachhang.FirstOrDefault();
                ViewBag.diachi = layDiaChi(k.MAXA, k.DAICHI);
                return View(khachhang.Single());
            }

        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(string id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                KHANHHANG khachhang = db.KHANHHANGs.SingleOrDefault(n => n.MAKH == id);
                db.KHANHHANGs.Remove(khachhang);
                db.SaveChanges();
                return RedirectToAction("Index", "KhachHang");
            }

        }
    }
}