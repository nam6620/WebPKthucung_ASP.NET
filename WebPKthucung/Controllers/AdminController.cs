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
    public class AdminController : Controller
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: Admin
        string LayMaAdmin()
        {
            var maMax = db.ADMINs.ToList().Select(n => n.MAADMIN).Max();
            int maAD = int.Parse(maMax.Substring(2)) + 1;
            string AD = String.Concat("000", maAD.ToString());
            return "AD" + AD.Substring(maAD.ToString().Length - 1);
        }
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
                return View();
        }
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult DSAdmin(int? page)
        { 
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            } else
            {
                if (page == null) page = 1;
                int pageSize = 7;
                var pageNumber = page ?? 1;
                var ad = from ADMIN in db.ADMINs select ADMIN;
                var list = ad.OrderBy(x => x.MAADMIN);
                return View(list.ToPagedList(pageNumber, pageSize));
            }
            
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var ad = from LOAITKADMIN in db.LOAITKADMINs select LOAITKADMIN;
                var maxPS = from THAMSO in db.THAMSOes where THAMSO.TENTHAMSO == "ĐỘ DÀI MẬT KHẨU" select THAMSO.SOLUONG;
                var minPS = from THAMSO in db.THAMSOes where THAMSO.TENTHAMSO == "MẬT KHẨU NGẮN NHẤT" select THAMSO.SOLUONG;
                var dsUserName = (from ADMIN in db.ADMINs select ADMIN.TENDN).ToArray();
                ViewBag.MAADMIN = LayMaAdmin();
                ViewBag.maxPassword = maxPS.FirstOrDefault();
                ViewBag.minPassword = minPS.FirstOrDefault();
                ViewBag.dsUserName = dsUserName;
                return View(ad);
            }    
                
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ADMIN admin, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/assest/img/ad_user/"), fileName);
                    fileUpload.SaveAs(path);
                    admin.MAADMIN = LayMaAdmin();
                    admin.AVATAR = fileName;
                    db.ADMINs.Add(admin);
                    db.SaveChanges();
                }
                return RedirectToAction("DSAdmin", "Admin");
            }
            
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var admin = from ad in db.ADMINs where ad.MAADMIN == id select ad;
                var list = from LOAITKADMIN in db.LOAITKADMINs select LOAITKADMIN;
                ViewBag.loaiTk = list;
                return View(admin.Single());
            }
               
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(String id, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.MAADMIN == id);
                if (fileUpload != null)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/assest/img/ad_user/"), fileName);
                    fileUpload.SaveAs(path);
                    ad.AVATAR = fileName;
                }
                UpdateModel(ad);
                db.SaveChanges();
                return RedirectToAction("DSAdmin", "Admin");
            }    
                
        }
        public ActionResult Detail(string id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var admin = from ad in db.ADMINs where ad.MAADMIN == id select ad;
                var list = from LOAITKADMIN in db.LOAITKADMINs select LOAITKADMIN;
                ViewBag.loaiTk = list;
                return View(admin.Single());
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
                var admin = from ad in db.ADMINs where ad.MAADMIN == id select ad;
                var list = from LOAITKADMIN in db.LOAITKADMINs select LOAITKADMIN;
                ViewBag.loaiTk = list;
                return View(admin.Single());
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
                ADMIN admin = db.ADMINs.SingleOrDefault(n => n.MAADMIN == id);
                db.ADMINs.Remove(admin);
                db.SaveChanges();
                return RedirectToAction("DSAdmin", "Admin");
            }
                
        }
        public ActionResult Search(int? page, string maadmin = "", string hoten = "", string dienthoai = "", string email = "", string maloai = "", string diachi = "")
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                ViewBag.loaiTk = from LOAITKADMIN in db.LOAITKADMINs select LOAITKADMIN;
                ViewBag.MAADMIN = maadmin;
                ViewBag.HOTEN = hoten;
                ViewBag.DIENTHOAI = dienthoai;
                ViewBag.EMAIL = email;
                int maloai_int;
                if (int.TryParse(maloai, out maloai_int))
                {
                    ViewBag.MALOAI = maloai_int;
                }
                else
                {
                    ViewBag.MALOAI = null;
                }
                ViewBag.DIACHI = diachi;
                var admins = db.ADMINs.AsQueryable();
                if (!string.IsNullOrEmpty(maadmin))
                {
                    admins = admins.Where(s => s.MAADMIN.Contains(maadmin));
                }
                if (!string.IsNullOrEmpty(hoten))
                {
                    admins = admins.Where(s => s.HOTEN.Contains(hoten));
                }
                if (!string.IsNullOrEmpty(dienthoai))
                {
                    admins = admins.Where(s => s.DIENTHOAI.Contains(dienthoai));
                }
                if (!string.IsNullOrEmpty(email))
                {
                    admins = admins.Where(s => s.EMAIL.Contains(email));
                }
                if (!string.IsNullOrEmpty(maloai))
                {
                    admins = admins.Where(s => s.MALOAI == maloai_int);
                }
                if (!string.IsNullOrEmpty(diachi))
                {
                    admins = admins.Where(s => s.DIACHI.Contains(diachi));
                }
                if (admins.Count() == 0)
                {
                    ViewBag.TB = "Không có thông tin tìm kiếm.";
                }
                else
                {
                    ViewBag.TB = null;
                }

                if (page == null) page = 1;
                int pageSize = 7;
                var pageNumber = page ?? 1;
                var list = admins.OrderBy(x => x.MAADMIN);
                return View(list.ToPagedList(pageNumber, pageSize));
            }   
                
        }
        [HttpGet]
        public ActionResult dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult dangnhap(DangNhapModel model)
        {
            if (ModelState.IsValid)
            {
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.TENDN == model.tendn && n.MATKHAU == model.matkhau);
                if (ad != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
            else
            {

            }
            return View(model);
        }
        public ActionResult dangxuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Admin");
        }
    }
}