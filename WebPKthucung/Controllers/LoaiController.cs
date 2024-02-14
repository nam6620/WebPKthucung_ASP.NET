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
    [AdminPhanQuyen(MACHUCNANG = "QL_LOAISANPHAM")]
    public class LoaiController : Controller
    {
        // GET: Loai
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        string LayMaLoai()
        {
            var maMax = db.LOAIs.ToList().Select(n => n.MALOAI).Max();
            int maMS = int.Parse(maMax.Substring(2)) + 1;
            string MS = String.Concat("00", maMS.ToString());
            return "LO" + MS.Substring(maMS.ToString().Length - 1);
        }       
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 10;
            var pageNumber = page ?? 1;
            var loais = from LOAI in db.LOAIs select LOAI;
            var list = loais.OrderBy(x => x.MALOAI);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaLoai = LayMaLoai();
            return View();
        }
        [HttpPost]
        public ActionResult Create(LOAI loai)
        {
            if (ModelState.IsValid)
            {
                loai.MALOAI = LayMaLoai();
                db.LOAIs.Add(loai);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Loai");
        }
        public ActionResult Delete(string id)
        {
            var loai = from l in db.LOAIs where l.MALOAI == id select l;
            return View(loai.Single());
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(string id)
        {
            LOAI loai = db.LOAIs.SingleOrDefault(n => n.MALOAI == id);
            db.LOAIs.Remove(loai);
            db.SaveChanges();
            return RedirectToAction("Index", "Loai");
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var loai = from l in db.LOAIs where l.MALOAI == id select l;
            return View(loai.Single());
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(String id)
        {
            LOAI loai = db.LOAIs.SingleOrDefault(n => n.MALOAI == id);
            UpdateModel(loai);
            db.SaveChanges();
            return RedirectToAction("Index", "Loai");
        }
    }
}