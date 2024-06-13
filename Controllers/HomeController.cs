using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;

namespace KQStore.Controllers
{
    public class HomeController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();
        public ActionResult Index()
        {
            ViewBag.CurrentPage = "Home";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(ContactFormModel model)
        {
            ViewBag.CurrentPage = "Contact";

            if (ModelState.IsValid)
            {
                try
                {
                    var body = $"From: {model.Name} ({model.Email})\nSubject: {model.Subject}\nMessage: {model.Message}";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(ConfigurationManager.AppSettings["EmailAdmin"]));  // Thay thế bằng email của bạn
                    message.Subject = "Khách hàng liên hệ";
                    message.Body = body;
                    message.IsBodyHtml = false;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = ConfigurationManager.AppSettings["Email"],  // Thay thế bằng email của bạn
                            Password = ConfigurationManager.AppSettings["PasswordEmail"]
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                    }

                    ViewBag.Message = "Gửi thành công!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Có lỗi xảy ra: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "Vui lòng điền đầy đủ thông tin.";
            }

            return View();
        }
    

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Login(string username, string password)
        {
            KQStoreEntities dd = new KQStoreEntities();
            string hashedPassword = GetMd5Hash(password);
            var nhanVien = dd.Users.SingleOrDefault(m => m.UserName == username && m.Password == hashedPassword);
            if (nhanVien != null)
            {

                Session["user"] = nhanVien;
                
                return Json(new { success = true, redirectTo = Url.Action("Index", "Home") });
            }
            else
            {
                
                return Json(new { success = false, error = "Tài khoản hoặc mật khẩu đăng nhập không đúng. Vui lòng thử lại" });
            }


        }
        public ActionResult Logout()
        {
            Session.Remove("user"); 
            //Session.Remove("cart");
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");

        }


        [AllowAnonymous]
        public ActionResult ValidateUsername(string username)
        {
            var existingUser = db.Users.FirstOrDefault(u => u.UserName == username);
            if (existingUser != null)
            {
                return Json(new { isValid = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isValid = true }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Name,Phone,Email,UserName,Password")] User user, string confirmPassword)
        {
            if (user.Password != confirmPassword)
            {
                return Json(new { success = false, errors = new[] { new { Key = "confirmPassword", ErrorMessage = "Mật khẩu xác nhận không khớp." } } });
            }

            var existingUser = db.Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                return Json(new { success = false, errors = new[] { new { Key = "username", ErrorMessage = "Tài khoản đã tồn tại." } } });
            }

            if (!IsValidEmail(user.Email))
            {
                return Json(new { success = false, errors = new[] { new { Key = "email", ErrorMessage = "Email không đúng định dạng." } } });
            }

            if (!IsValidPhone(user.Phone))
            {
                return Json(new { success = false, errors = new[] { new { Key = "phone", ErrorMessage = "Số điện thoại phải có đủ 10 số." } } });
            }

            if (!IsValidPassword(user.Password))
            {
                return Json(new { success = false, errors = new[] { new { Key = "password", ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự gồm chữ và số." } } });
            }

            user.RoleId = 2;

            if (ModelState.IsValid)
            {
                user.Password = GetMd5Hash(user.Password);
                user.CreatedAt = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                return Json(new { success = true, redirectTo = Url.Action("Login", "Home") });
            }

            return Json(new { success = false, errorMessage = "Đã xảy ra lỗi không xác định." });
        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = new System.Text.RegularExpressions.Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            return emailPattern.IsMatch(email);
        }

        private bool IsValidPhone(string phone)
        {
            var phonePattern = new System.Text.RegularExpressions.Regex(@"^\d{10}$");
            return phonePattern.IsMatch(phone);
        }

        private bool IsValidPassword(string password)
        {
            var passwordPattern = new System.Text.RegularExpressions.Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$");
            return passwordPattern.IsMatch(password);
        }

        private string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Chuyển đổi input thành mảng byte và băm
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Tạo một StringBuilder để lưu trữ kết quả băm
                StringBuilder sBuilder = new StringBuilder();

                // Chuyển đổi mỗi byte trong mảng đã băm thành một chuỗi hexa và thêm vào StringBuilder
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Trả về chuỗi hexa đã băm
                return sBuilder.ToString();
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}