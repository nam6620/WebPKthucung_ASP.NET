using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebPKthucung.Models;

namespace WebPKthucung.Controllers
{
    public class ThuongHieuController : Controller
    {
        // GET: ThuongHieu
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        string LayMaThuongHieu()
        {
            var maMax = db.THUONGHIEUx.ToList().Select(n => n.MATH).Max();
            int maTH;
            if (maMax == null)
            {
                maTH = 1;
            }
            else
            {
                maTH = int.Parse(maMax.Substring(2)) + 1;
            }
            string TH = String.Concat("00", maTH.ToString());
            return "TH" + TH.Substring(maTH.ToString().Length - 1);
        }
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 7;
            var pageNumber = page ?? 1;
            var thuongHieus = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            var list = thuongHieus.OrderBy(x => x.MATH);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaThuongHieu = LayMaThuongHieu();
            return View();
        }
        [HttpPost]
        public ActionResult Create(THUONGHIEU thuonghieu, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/assest/img/thuong_hieu/"), fileName);
                fileUpload.SaveAs(path);
                thuonghieu.MATH = LayMaThuongHieu();
                thuonghieu.LOGO = fileName;
                db.THUONGHIEUx.Add(thuonghieu);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "ThuongHieu");
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var thuonghieu = from th in db.THUONGHIEUx where th.MATH == id select th;
            return View(thuonghieu.Single());
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(String id, HttpPostedFileBase fileUpload)
        {
            THUONGHIEU th = db.THUONGHIEUx.SingleOrDefault(n => n.MATH == id);
            if (fileUpload != null)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/assest/img/thuong_hieu/"), fileName);
                fileUpload.SaveAs(path);
                th.LOGO = fileName;
            }
            UpdateModel(th);
            db.SaveChanges();
            return RedirectToAction("Index", "ThuongHieu");
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var thuonghieu = from th in db.THUONGHIEUx where th.MATH == id select th;
            return View(thuonghieu.Single());
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(string id)
        {
            THUONGHIEU thuonghieu = db.THUONGHIEUx.SingleOrDefault(n => n.MATH == id);
            db.THUONGHIEUx.Remove(thuonghieu);
            db.SaveChanges();
            return RedirectToAction("Index", "SanPham");
        }
    }
}