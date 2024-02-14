using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPKthucung.App_Start;
using WebPKthucung.Models;

namespace WebPKthucung.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_SANPHAM")]
    public class SanPhamController : Controller
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: SanPham
        string LayMaSanPham()
        {
            var maMax = db.SANPHAMs.ToList().Select(n => n.MASP).Max();
            int maMA;
            if (maMax == null)
            {
                maMA = 1;
            }
            else
            {
                maMA = int.Parse(maMax.Substring(2)) + 1;
            }
            string AD = String.Concat("000000", maMA.ToString());
            return "SP" + AD.Substring(maMA.ToString().Length - 1);
        }
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 7;
            var pageNumber = page ?? 1;
            var sanphams = from SANPHAM in db.SANPHAMs select SANPHAM;
            var list = sanphams.OrderBy(x => x.MASP);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.sanpham = LayMaSanPham();
            ViewBag.ThuongHieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            ViewBag.Loai = from LOAI in db.LOAIs select LOAI;
            ViewBag.MauSac = from MAUSAC in db.MAUSACs select MAUSAC;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SANPHAM sanpham, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/assest/img/sanpham/"), fileName);
                ViewBag.ThuongHieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
                ViewBag.Loai = from LOAI in db.LOAIs select LOAI;
                ViewBag.MauSac = from MAUSAC in db.MAUSACs select MAUSAC;
                fileUpload.SaveAs(path);
                sanpham.MASP = LayMaSanPham();
                sanpham.HINHANH = fileName;
                sanpham.SOLUONG = 0;
                db.SANPHAMs.Add(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index", "SanPham");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.ThuongHieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            ViewBag.Loai = from LOAI in db.LOAIs select LOAI;
            ViewBag.MauSac = from MAUSAC in db.MAUSACs select MAUSAC;
            var sanpham = from SANPHAM in db.SANPHAMs where SANPHAM.MASP == id select SANPHAM;
            return View(sanpham.Single());
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(String id, HttpPostedFileBase fileUpload)
        {
            SANPHAM sanpham = db.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            if (fileUpload != null)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/assest/img/sanpham/"), fileName);
                fileUpload.SaveAs(path);
                sanpham.HINHANH = fileName;
            }
            UpdateModel(sanpham);
            db.SaveChanges();
            return RedirectToAction("Index", "SanPham");
        }
        public ActionResult Detail(string id)
        {
            var sanpham = from SANPHAM in db.SANPHAMs where SANPHAM.MASP == id select SANPHAM;
            ViewBag.ThuongHieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            ViewBag.Loai = from LOAI in db.LOAIs select LOAI;
            ViewBag.MauSac = from MAUSAC in db.MAUSACs select MAUSAC;
            return View(sanpham.Single());
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var sanpham = from SANPHAM in db.SANPHAMs where SANPHAM.MASP == id select SANPHAM;
            ViewBag.ThuongHieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            ViewBag.Loai = from LOAI in db.LOAIs select LOAI;
            ViewBag.MauSac = from MAUSAC in db.MAUSACs select MAUSAC;
            return View(sanpham.Single());
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(string id)
        {
            SANPHAM sanpham = db.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            db.SANPHAMs.Remove(sanpham);
            db.SaveChanges();
            return RedirectToAction("Index", "SanPham");
        }
    }
}