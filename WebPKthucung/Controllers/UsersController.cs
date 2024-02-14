using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebPKthucung.Models;
using System.Security.Cryptography;
using System.Text;
using System.Drawing.Printing;

namespace WebPKthucung.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        QLPKTHUCUNGEntities1 db = new QLPKTHUCUNGEntities1();
        string LayMa()
        {
            var maMax = db.KHANHHANGs.ToList().Select(n => n.MAKH).Max();
            int maAD;
            if (maMax == null)
            {
                maAD = 1;
            }
            else
            {
                maAD = int.Parse(maMax.Substring(2)) + 1;
            }
            string AD = String.Concat("000000", maAD.ToString());
            return "KH" + AD.Substring(maAD.ToString().Length - 1);
        }
        string maHoaM5(String mk)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] mahoamd5;
            UTF8Encoding encode = new UTF8Encoding();
            mahoamd5 = md5.ComputeHash(encode.GetBytes(mk));
            StringBuilder data = new StringBuilder();
            for (int i = 0; i < mahoamd5.Length; i++)
            {
                data.Append(mahoamd5[i].ToString("x2"));
            }
            return data.ToString();
        }
        bool checkGiamGia( string MaSP)
        {
            GIAMGIA giamgia = db.GIAMGIAs.SingleOrDefault(n => n.MASP == MaSP && n.NGAYBATDAU <= DateTime.Now && n.NGAYKETTHUC >= DateTime.Now);
            if (giamgia != null)
            {
                return true;
            }
            return false;
        }
        string layDiaChi(String id)
        {
            KHANHHANG kh = db.KHANHHANGs.FirstOrDefault(n => n.MAKH == id);
            XA xa = db.XAs.FirstOrDefault(n => n.MAXA == kh.MAXA);
            HUYEN huyen = db.HUYENs.FirstOrDefault(n => n.MAHUYEN == xa.MAHUYEN);
            TINH tinh = db.TINHs.FirstOrDefault(n => n.MATINH == huyen.MATINH);
            string diaChi =" đường "+ kh.DAICHI + " xã " + xa.TENTINH + " huyện " + huyen.TENTINH + "tỉnh" + tinh.TENTINH;
            //Console.WriteLine("maXa = {0}, tenDuong = {1}, xa.TENTINH = {2}, huyen.TENTINH = {3}, tinh.TENTINH = {4}, diaChi = {5}", maXa, tenDuong, xa.TENTINH, huyen.TENTINH, tinh.TENTINH, diaChi);
            return diaChi;
        }
        public ActionResult Index()
        {
            var allacc = (from a in db.SANPHAMs
                          join b in db.THUONGHIEUx on a.MATH equals b.MATH
                          join c in db.LOAIs on a.MALOAI equals c.MALOAI
                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = a.SOLUONG,
                              MOTA = a.MOTA,

                          }).Take(count: 4).OrderBy(x => x.MASP).ToList();
            foreach (var product in allacc)
            {
                product.GIAMGIA = checkGiamGia(product.MASP);
            }
            ViewBag.ThuongHieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            return View(allacc);
        }
        public ActionResult sanpham(int? page)
        {
            if (page == null) page = 1;
            //Số mẫu tin tối đa cho 1 trang
            int pagesize = 9;
            //Nếu biến page là null thì pagenum=1, ngược pagenum = page.
            int pagenum = (page ?? 1);
            var allacc = (from a in db.SANPHAMs
                          join b in db.THUONGHIEUx on a.MATH equals b.MATH
                          join c in db.LOAIs on a.MALOAI equals c.MALOAI
                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = a.SOLUONG,
                              MOTA = a.MOTA,
                          }).OrderBy(x => x.MASP);
            foreach (var product in allacc)
            {
                product.GIAMGIA = checkGiamGia(product.MASP);
            }
            return View(allacc.ToPagedList(pagenum, pagesize)); 
        }
        [HttpGet]
        public ActionResult dangnhap()
        {
            return View();
        }


        #region Đăng nhập tài khoản người dùng
        [HttpPost]
        public ActionResult dangnhap(DangNhapModel model, string currentUrl)
        {
            
            var mahoa_matkhaudangnhap = maHoaM5(model.matkhau);
            if (ModelState.IsValid)
            {
                KHANHHANG kh = db.KHANHHANGs.SingleOrDefault(n => n.TENDNKH == model.tendn && n.MATKHAUKH == mahoa_matkhaudangnhap);
                if (kh != null)
                {
                    Session["LoginSai"] = null;
                    Session["Taikhoan"] = kh;
                    Session["DiaChi"] = layDiaChi(kh.MAKH);
                    return RedirectToAction("index", "Users"); 
                }
                else
                    Session["LoginSai"] = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            else
            {

            }
            return View();
        }
        #endregion
        [HttpGet]
        public ActionResult dangky()
        {
            var maxPS = from THAMSO in db.THAMSOes where THAMSO.TENTHAMSO == "ĐỘ DÀI MẬT KHẨU" select THAMSO.SOLUONG;
            var minPS = from THAMSO in db.THAMSOes where THAMSO.TENTHAMSO == "MẬT KHẨU NGẮN NHẤT" select THAMSO.SOLUONG;
            var dsTK = (from KHANHHANG in db.KHANHHANGs select KHANHHANG.TENDNKH).ToArray();
            var dsSDT = (from KHANHHANG in db.KHANHHANGs select KHANHHANG.DIENTHOAI).ToArray();
            var dsEmail = (from KHANHHANG in db.KHANHHANGs select KHANHHANG.EMAIL).ToArray();
            ViewBag.dsTinh = (from TINH in db.TINHs select TINH).ToArray();
            ViewBag.dsHuyen = (from HUYEN in db.HUYENs select HUYEN).ToArray();
            ViewBag.dsXa = (from XA in db.XAs select XA).ToArray();
            ViewBag.maxPassword = maxPS.FirstOrDefault();
            ViewBag.minPassword = minPS.FirstOrDefault();
            ViewBag.dsTaiKhoan = dsTK;
            ViewBag.dsSDT = dsSDT;
            ViewBag.dsEmail = dsEmail;
            return View();
        }
        [HttpPost]
        public ActionResult dangky(KHANHHANG khachhang, HttpPostedFileBase fileUpload)

        {
            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/assest/img/khach_hang/"), fileName);
                    fileUpload.SaveAs(path);
                    khachhang.HINHANH = fileName;
                }    
                var mahoa_matkhau = maHoaM5(khachhang.MATKHAUKH);
                khachhang.MAKH = LayMa();
                khachhang.MATKHAUKH = mahoa_matkhau;
                db.KHANHHANGs.Add(khachhang);
                db.SaveChanges();          
            }
            return RedirectToAction("sanpham");
        }
        [HttpPost]
        public JsonResult GetXaList()
        {
            try
            {
                List<XA> xaList = new List<XA>();
                xaList = db.XAs.Where(x => x.MAHUYEN == "DL").ToList();
                SelectList xaSelectList = new SelectList(xaList, "XaID", "TenXa", 0);
                return Json(new { success = true, data = xaSelectList }, JsonRequestBehavior.AllowGet);
            } catch (Exception ex)
            {
                return Json(new { success = true, data =  ex.Message}, JsonRequestBehavior.AllowGet);
            }
            
        }
        #region Kiểm tra thông tin cá nhân của tài khoản đăng nhập
        public ActionResult thongtincanhan()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("index", "Users");

            }
            return PartialView();
        }
        #endregion

        [HttpGet]
        public ActionResult QuenMK()
        {
            return View();
        }
        #region Chức năng quên mật khẩu
        [HttpPost]
        public ActionResult QuenMK(String quenMK)
        {
            if (ModelState.IsValid)
            {
                //Xác nhận email
                //tạo liên kết đặt lại mật khẩu
                //gửi email               

                var account = db.KHANHHANGs.SingleOrDefault(n => n.EMAIL == quenMK);
                if (account != null)
                {
                    //gửi mail để thay đổi mật khẩu

                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EMAIL, resetCode, "ResetPassword");
                    account.KHOIPHUCMK = resetCode;
                    db.SaveChanges();
                    ViewBag.TB = "Đã gửi mail xác nhận cập nhật tài khoản";
                    return View();
                }
                else
                {
                    ViewBag.message = "Email không đúng";
                }

            }
            else
            {

            }
            return View(quenMK);
        }
        #endregion
        

        #region Gửi liên kết xác minh thay đổi mật khẩu về mail khách hàng yêu cầu
        [NonAction]
        public void SendVerificationLinkEmail(string emailId, string activationCode, string emailFor = "ResetPassword")
        {
            var verifyUrl = "/Users/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("nam5520000@gmail.com", "PaddyShop");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "bsstoatsilpibrdm"; // replace with actual password
            string subject = "";
            string body = "";
            if (emailFor == "ResetPassword")
            {
                subject = "Đặt lại mật khẩu";
                body = "<b>Xin chào bạn</b>,<br/><br/> Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn. Vui lòng nhấp vào liên kết dưới đây để thiết lập mật khẩu mới cho tài khoản của bạn " + "<br/><br/><a href=" + link + ">Link đặt lại mật khẩu</a>";
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
        #endregion
        #region Thay đổi mật khẩu thành công
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            KHANHHANG kh = db.KHANHHANGs.SingleOrDefault(n => n.KHOIPHUCMK == id);
            if (kh != null)
            {
                ResetPassword model = new ResetPassword();
                model.Resetcode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                KHANHHANG kh = db.KHANHHANGs.SingleOrDefault(n => n.KHOIPHUCMK == model.Resetcode);
                if (kh != null)
                {
                    var mahoa_matkhau = maHoaM5(model.NewPassword);
                    kh.MATKHAUKH = mahoa_matkhau;
                    kh.KHOIPHUCMK = "";
                    UpdateModel(kh);
                    db.SaveChanges();
                    ViewBag.TB = "Cập nhật mật khẩu mới thành công";
                }
            }
            else
            {
                message = "Điều gì đó không hợp lệ";
            }
            ViewBag.Message = message;
            return View(model);
        }
        #endregion

        public ActionResult GetDistricts(string maTinh)
        {
            try
            {
               
                var listHuyen = (from a in db.HUYENs where a.MATINH == maTinh
                                 select new getHuyen
                {
                    MAHUYEN =a.MAHUYEN,
                    TENHUYEN = a.TENTINH
                }).ToList();

                // Trả về danh sách quận/huyện dưới dạng JSON
                return Json(new { success = true, data = listHuyen }, JsonRequestBehavior.AllowGet);
            }
            catch  (Exception ex)
            {
                // Xử lý nếu có lỗi xảy ra
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            // Lấy danh sách quận/huyện theo tỉnh/thành phố
        }
        public ActionResult GetXa(string maHuyen)
        {
            try
            {

                var listHuyen = (from a in db.XAs
                                 where a.MAHUYEN == maHuyen
                                 select new getXa
                                 {
                                     MAXA = a.MAXA,
                                     TENXA = a.TENTINH
                                 }).ToList();

                // Trả về danh sách quận/huyện dưới dạng JSON
                return Json(new { success = true, data = listHuyen }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý nếu có lỗi xảy ra
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            // Lấy danh sách quận/huyện theo tỉnh/thành phố
        }
        public ActionResult QuenMKxacnhan()
        {
            return View();
        }
        public ActionResult Ketquatimkiem(string searchString)
        {

            var allacc = (from a in db.SANPHAMs
                          join b in db.THUONGHIEUx on a.MATH equals b.MATH
                          join c in db.LOAIs on a.MALOAI equals c.MALOAI
                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = a.SOLUONG,
                              MOTA = a.MOTA,
                          });
            if (!String.IsNullOrEmpty(searchString)) /*Nếu không phải trống thì lấy ra sản phẩm có tên với từ khóa tìm kiếm*/
            {
                allacc = allacc.Where(s => s.TENSP.Contains(searchString));
            }
            
            foreach (var product in allacc)
            {
                product.GIAMGIA = checkGiamGia(product.MASP);
            }
            ViewBag.THUONGHIEU = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            allacc.ToList();
            //Trả về tất cả sản phẩm
            return View(allacc);
        }
        public ActionResult chitiet(string id)
        {
            var allacc = (from a in db.SANPHAMs
                          join b in db.THUONGHIEUx on a.MATH equals b.MATH
                          join c in db.LOAIs on a.MALOAI equals c.MALOAI
                          where a.MASP == id
                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = a.SOLUONG,
                              MOTA = a.MOTA,


                          });

            foreach (var product in allacc)
            {
                product.GIAMGIA = checkGiamGia(product.MASP);
            }
            return View(allacc.SingleOrDefault());
        }
        [NonAction]
        public void sendcontact(string Name, string Email, string Subject, string Content)
        {
            KHANHHANG kh = new KHANHHANG();
            var fromEmail = new MailAddress("nam5520000@gmail.com");
            var toEmail = new MailAddress(kh.EMAIL);
            var fromEmailPassword = "Ppn978421356"; // replace with actual password
            string subject = Subject;
            string body = "<br/> Họ tên: " + Name + "<br/><br/> Email: " + " " + Email + "<br/><br/> Nội dung: " + Content;

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
        public ActionResult Lienhe()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Lienhe(LienheModel lienhe)
        {
            if (ModelState.IsValid)
            {
                sendcontact(lienhe.Name, lienhe.Email, lienhe.Subject, lienhe.Message);
                ViewBag.TB = "Gửi email thành công. Chúng tôi sẻ liên hệ với bạn trong thời gian sớm nhất.";
               
            }
            return View(lienhe);
        }


        public ActionResult thongbaolienhe()
        {
            return View();
        }

        public ActionResult blogs()
        {
            return View();
        }
        public ActionResult GioiThieu()
        {
            return View();
        }

        public ActionResult ThuongHieu()
        {
            var thuongHieus = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            return View(thuongHieus);
        }
        public ActionResult th()
        {
            var listthuonghieu = from THUONGHIEU in db.THUONGHIEUx select THUONGHIEU;
            return PartialView(listthuonghieu);
        }
        public ActionResult thongTinDiaChi(String id)
        {
            String diaChi = layDiaChi(id);
            return PartialView(diaChi);
        }
        public ActionResult loai()
        {
            var listloai = from LOAI in db.LOAIs select LOAI;
            return PartialView(listloai);
        }
        public ActionResult SPTheothuonghieu(int? page,string id)
        {
            if (page == null) page = 1;
            //Số mẫu tin tối đa cho 1 trang
            int pagesize = 9;
            var tenTH = (from THUONGHIEU in db.THUONGHIEUx where THUONGHIEU.MATH == id select THUONGHIEU.TENTH);
            ViewBag.ThuongHieu = tenTH.FirstOrDefault();
            //Nếu biến page là null thì pagenum=1, ngược pagenum = page.
            int pagenum = (page ?? 1);
            var allacc = (from a in db.SANPHAMs
                          join b in db.THUONGHIEUx on a.MATH equals b.MATH 
                          join c in db.LOAIs on a.MALOAI equals c.MALOAI
                          where b.MATH == id
                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = a.SOLUONG,
                              MOTA = a.MOTA,
                          }).OrderBy(x => x.MASP);
            foreach (var product in allacc)
            {
                product.GIAMGIA = checkGiamGia(product.MASP);
            }
            return View(allacc.ToPagedList(pagenum, pagesize));
        }
        public ActionResult SPTheoloai(int? page, String id)
        {
            if (page == null) page = 1;
            //Số mẫu tin tối đa cho 1 trang
            int pagesize = 9;
            var tenloai = (from LOAI in db.LOAIs where LOAI.MALOAI == id select LOAI.TENLOAI);
            ViewBag.loai = tenloai.FirstOrDefault();
            //Nếu biến page là null thì pagenum=1, ngược pagenum = page.
            int pagenum = (page ?? 1);
            var allacc = (from a in db.SANPHAMs
                          join b in db.THUONGHIEUx on a.MATH equals b.MATH
                          join c in db.LOAIs on a.MALOAI equals c.MALOAI
                          where c.MALOAI == id
                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = a.SOLUONG,
                              MOTA = a.MOTA,

                          }).OrderBy(x => x.MASP);
            foreach (var product in allacc)
            {
                product.GIAMGIA = checkGiamGia(product.MASP);
            }
            return View(allacc.ToPagedList(pagenum, pagesize));
        }
        public ActionResult SPtheoGia(int? page, int min, int max)
        {
            if (page == null) page = 1;
            //Số mẫu tin tối đa cho 1 trang
            int pagesize = 9;
            //Nếu biến page là null thì pagenum=1, ngược pagenum = page.
            int pagenum = (page ?? 1);
            var allacc = (from a in db.SANPHAMs
                          join b in db.THUONGHIEUx on a.MATH equals b.MATH
                          join c in db.LOAIs on a.MALOAI equals c.MALOAI
                          where a.DONGIABAN >= min && a.DONGIABAN <= max
                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = a.SOLUONG,
                              MOTA = a.MOTA,

                          }).OrderBy(x => x.MASP);
            foreach (var product in allacc)
            {
                product.GIAMGIA = checkGiamGia(product.MASP);
            }
            return View(allacc.ToPagedList(pagenum, pagesize));
        }
        public ActionResult dangxuat()
        {
            Session.Clear();
            return RedirectToAction("index", "Users");
        }

    }
}