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
    [AdminPhanQuyen(MACHUCNANG = "QL_KHOSANPHAM")]
    public class PhieuNhapKhoController : Controller
       
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: PhieuNhapKho
        string LayMa()
        {
            var maMax = db.PHIEUNHAPKHOes.ToList().Select(n => n.MAPHIEUNK).Max();
            int maMS;
            if (maMax == null)
            {
                maMS = 1;
            }
            else
            {
                maMS = int.Parse(maMax.Substring(2)) + 1;
            }
            string MS = String.Concat("00", maMS.ToString());
            return "PN" + MS.Substring(maMS.ToString().Length - 1);
        }
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 7;
            var pageNumber = page ?? 1;
            var masacs = from PHIEUNHAPKHO in db.PHIEUNHAPKHOes select PHIEUNHAPKHO;
            var list = masacs.OrderBy(x => x.MAPHIEUNK);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MAPK = LayMa();
            ViewBag.SANPHAM = from SANPHAM in db.SANPHAMs select SANPHAM;
            ViewBag.MAPHIEUNHAP = LayMa();
            return View();
        }
        [HttpPost]
        public ActionResult Create(PHIEUNHAPKHO kho)
        {
            SANPHAM sanpham = db.SANPHAMs.Single(n => n.MASP == kho.MASP);
            sanpham.SOLUONG += kho.SOLUONG;
            db.PHIEUNHAPKHOes.Add(kho);
            db.SaveChanges();
            return RedirectToAction("Index", "PhieuNhapKho");
        }

    }
}