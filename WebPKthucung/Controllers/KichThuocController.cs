using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebPKthucung.App_Start;
using WebPKthucung.Models;

namespace WebPKthucung.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_KICHTHUOC")]
    public class KichThuocController : Controller
    {
        // GET: KichThuoc
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 7;
            var pageNumber = page ?? 1;
            var loais = from LOAI in db.KICHTHUOCs select LOAI;
            var list = loais.OrderBy(x => x.MAKICHTHUOC);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(LOAI loai)
        {
            if (ModelState.IsValid)
            {
                db.LOAIs.Add(loai);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Loai");
        }
    }
}