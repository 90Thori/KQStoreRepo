using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace KQStore.Areas.Admin.Controllers
{
    public class HomesController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAdmin(string username, string password)
        {
            KQStoreEntities dd = new KQStoreEntities();
            string hashedPassword = GetMd5Hash(password);
            var admin = dd.Users.SingleOrDefault(m => m.UserName.ToLower() == username.ToLower() && m.Password == hashedPassword);
            if(admin!=null && admin.RoleId==2)
            {
                ViewBag.Error  = "Bạn không có quyền truy cập";
                return View();
            }    
            if (admin != null )
            {
                Session["user"] = admin;
                //ViewBag.user = nhanVien;
                return RedirectToAction("Index", "Homes");
            }
            else 
            {
                ViewBag.Error = "Tài khoản đăng nhập không đúng.";
                return View();
            }


        }
        public ActionResult Logout()
        {
            Session.Remove("user");
            //Session.Remove("cart");
            FormsAuthentication.SignOut();
            return RedirectToAction("LoginAdmin", "Homes");

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
    }
}