using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebPKthucung.App_Start;
using WebPKthucung.Models;

namespace WebPKthucung.Controllers
{
    
    [AdminPhanQuyen(MACHUCNANG = "QL_DONDATHANG")]
    public class DonHangController : Controller
    {
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        // GET: DonHang
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
                var dh = from DONDATHANG in db.DONDATHANGs select DONDATHANG;
                var list = dh.OrderBy(x => x.MADH);
                return View(list.ToPagedList(pageNumber, pageSize));
            }
            
        }
        public ActionResult ChiTietDonHang(string id)
        {
            if (Session["Taikhoanadmin"] == null)//Chưa đăng nhập => Login
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MADH == id);
                ViewBag.ten = db.KHANHHANGs.SingleOrDefault(n => n.MAKH == ddh.MAKH).HOTENKH;
                ViewBag.tenDongHang = ddh.MADH; 
                var CTDH = (from c in db.CTDONDATHANGs where c.MADH == id select c).ToList();
                return View(CTDH);
            }
        }
        [NonAction]
        public void SendEmail(string emailId, string id, string emailFor = "SendDonHang")
        {
            var fromEmail = new MailAddress("nam5520000@gmail.com", "PaddyShop");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "bsstoatsilpibrdm"; // replace with actual password
            string subject = "";
            string body = "";
            string m = "";
            DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MADH == id);
            var CTDH = (from c in db.CTDONDATHANGs where c.MADH == id select c).ToList();
            if (emailFor == "SendDonHang")
            {
                subject = "Đơn đặt hàng";
                body = "<b>Xin chào bạn</b>,<br/><br/> Đơn đặt hàng của bạn đã đc xác nhận.<br/>Số tiền cần thanh toán là:"+ddh.TONGTIEN+ "<br/>"+"Chi tiết đơn hàng gồm";
                foreach (var item in CTDH)
                {
                    SANPHAM sanpham = db.SANPHAMs.Single(n => n.MASP == item.MASP);
                    m = "<br/>" + sanpham.TENSP + " số lượng: " + item.SOLUONG + " thành tiền: " + item.THANHTIEN;
                    body += m;
                }

            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) smtp.Send(message);
        }
        [HttpGet]
        public ActionResult XacNhanDonDatHang(string id)
        {
            if (Session["Taikhoanadmin"] == null)//Chưa đăng nhập => Login
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MADH == id);
                ViewBag.ten = db.KHANHHANGs.SingleOrDefault(n => n.MAKH == ddh.MAKH).HOTENKH;
                ViewBag.tenDongHang = ddh.MADH;
                var CTDH = (from c in db.CTDONDATHANGs where c.MADH == id select c).ToList();
                return View(CTDH);
            }
        }
        [HttpPost, ActionName("XacNhanDonDatHang")]
        public ActionResult XacNhan(string id)
        {
            if (Session["Taikhoanadmin"] == null)//Chưa đăng nhập => Login
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MADH == id);
                ViewBag.ten = db.KHANHHANGs.SingleOrDefault(n => n.MAKH == ddh.MAKH).HOTENKH;
                ViewBag.tenDongHang = ddh.MADH;
                var CTDH = (from c in db.CTDONDATHANGs where c.MADH == id select c).ToList();
                foreach (var item in CTDH)
                {
                    SANPHAM sanpham = db.SANPHAMs.Single(n => n.MASP == item.MASP);
                    if (sanpham.SOLUONG >= item.SOLUONG)
                    {
                        sanpham.SOLUONG -=  item.SOLUONG;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.TB = "Đơn hàng không thành công";
                        return View();
                    }
                }
                KHANHHANG kh = db.KHANHHANGs.SingleOrDefault(n => n.MAKH == ddh.MAKH);
                SendEmail(kh.EMAIL, id);
                ddh.TINHTRANGDH = true;
                db.SaveChanges();
                return RedirectToAction("Index", "DonHang"); ;
            }
        }
    }
}