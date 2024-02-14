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
    [AdminPhanQuyen(MACHUCNANG = "QL_PHANQUYEN")]
    public class AdminPQController : Controller
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: AdminPQ
        public ActionResult Index(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                if (page == null) page = 1;
                int pageSize = 16;
                var pageNumber = page ?? 1;
                var ad = from ADMIN in db.ADMINs select ADMIN;
                var list = ad.OrderBy(x => x.MAADMIN);
                return View(list.ToPagedList(pageNumber, pageSize));
            }
           
        }
        public ActionResult ChiTietDSPhanQuyen(String id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var ad = from PHANQUYEN in db.PHANQUYENs where PHANQUYEN.MAADMIN == id select PHANQUYEN;
                var ten = from ADMIN in db.ADMINs where ADMIN.MAADMIN == id select ADMIN.HOTEN;
                var ltk = from ADMIN in db.ADMINs where ADMIN.MAADMIN == id select ADMIN.LOAITKADMIN.TENLOAI;
                ViewBag.hoTenAdmin = ten.FirstOrDefault();
                ViewBag.ltk = ltk.FirstOrDefault();
                return View(ad);
            }
        }
        public ActionResult DSChucNang()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var ad = from CHUCNANG_QUYEN in db.CHUCNANG_QUYEN select CHUCNANG_QUYEN;
                return View(ad);
            }
        }
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                ViewBag.CHUCNANG = from CHUCNANG_QUYEN in db.CHUCNANG_QUYEN select CHUCNANG_QUYEN;
                ViewBag.ADMIN = from ADMIN in db.ADMINs select ADMIN;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(PHANQUYEN phanquyen)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                db.PHANQUYENs.Add(phanquyen);
                db.SaveChanges();
                return RedirectToAction("Index", "AdminPQ");
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in db.PHANQUYENs where m.MAPQ == id select m;
                return View(mau.Single());
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                PHANQUYEN mau = db.PHANQUYENs.SingleOrDefault(n => n.MAPQ == id);
                db.PHANQUYENs.Remove(mau);
                db.SaveChanges();
                return RedirectToAction("Index", "AdminPQ");
            }
        }

    }
}   