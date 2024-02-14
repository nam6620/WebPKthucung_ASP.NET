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
    [AdminPhanQuyen(MACHUCNANG = "QL_MAUSAC")]

    public class MauSacController : Controller
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: MauSac
        string LayMaMauSac()
        {
            var maMax = db.MAUSACs.ToList().Select(n => n.MAMAUSAC).Max();
            int maMS = int.Parse(maMax.Substring(2)) + 1;
            string MS = String.Concat("00", maMS.ToString());
            return "MS" + MS.Substring(maMS.ToString().Length - 1);
        }
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 7;
            var pageNumber = page ?? 1;
            var masacs = from MAUSAC in db.MAUSACs select MAUSAC;
            var list = masacs.OrderBy(x => x.MAMAUSAC);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaMauSac = LayMaMauSac();
            return View();
        }
        [HttpPost]
        public ActionResult Create(MAUSAC mausac)
        {
            if (ModelState.IsValid)
            {
                mausac.MAMAUSAC = LayMaMauSac();
                db.MAUSACs.Add(mausac);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "MauSac");
        }
        public ActionResult Delete(string id)
        {
            var mausac = from ms in db.MAUSACs where ms.MAMAUSAC == id select ms;
            return View(mausac.Single());
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(string id)
        {
            MAUSAC mausac = db.MAUSACs.SingleOrDefault(n => n.MAMAUSAC == id);
            db.MAUSACs.Remove(mausac);
            db.SaveChanges();
            return RedirectToAction("Index", "MauSac");
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var mausac = from ms in db.MAUSACs where ms.MAMAUSAC == id select ms;
            return View(mausac.Single());
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(String id)
        {
            MAUSAC mausac = db.MAUSACs.SingleOrDefault(n => n.MAMAUSAC == id);
            UpdateModel(mausac);
            db.SaveChanges();
            return RedirectToAction("Index", "MauSac");
        }
    }
}