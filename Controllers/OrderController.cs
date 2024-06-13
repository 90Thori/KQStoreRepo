using KQStore.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class OrderController : Controller
    {
        private KQStoreEntities db= new KQStoreEntities();
        // GET: Order
        public ActionResult Index()
        {

            var user = Session["user"] as User;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var cart = db.Carts.SingleOrDefault(c => c.UserId == user.UserId);
            

            var cartItems = db.CartItems.Where(c => c.CartId == cart.CartId).Include(c => c.Product).ToList();
            var userCheck = db.Users.SingleOrDefault(u => u.UserId == user.UserId);
            var viewModel = new CheckoutViewModel
            {
                User = userCheck,
                CartItems = cartItems
            };

            return View(viewModel);
        }
        public ActionResult UserOrder() {
            var user= Session["user"];

            return PartialView("_UserOrder", user);
        }
        public ActionResult PlaceOrder(IEnumerable<CartItem> item, int total, string note, string address)
        {
            var user = Session["user"] as User;
            if (string.IsNullOrEmpty(address))
            {
                ViewBag.Error = "Bạn cần nhập địa chỉ";

                var cartt = db.Carts.SingleOrDefault(c => c.UserId == user.UserId);
                var cartItemst = db.CartItems.Where(c => c.CartId == cartt.CartId).Include(c => c.Product).ToList();
                var userCheckt = db.Users.SingleOrDefault(u => u.UserId == user.UserId);

                var viewModel = new CheckoutViewModel
                {
                    User = userCheckt,
                    CartItems = cartItemst
                };

                return View("Index", viewModel);
            }

            Order order = new Order
            {
                OrderDate = DateTime.Now,
                Total = total,
                Note = note,
                IdPay = 1,
                UserId = user.UserId,                        
                OrderCode = "DH" + new Random().Next(1000, 9999),
                ShipAddress= address
            };
            var userCheck = db.Users.SingleOrDefault(u => u.UserId == user.UserId);
            
            if (userCheck.Address == null)
            {
                
                userCheck.Address = address;
                
                db.Entry(userCheck).State = EntityState.Modified;
                db.SaveChanges();
            }
            

            db.Orders.Add(order);
            db.SaveChanges();
            var cart = db.Carts.SingleOrDefault(c => c.UserId == user.UserId);

            var cartItems = db.CartItems.Where(c => c.CartId == cart.CartId).Include(c => c.Product).ToList();
            
            foreach (var it in cartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = it.ProductId,
                    Quantity = it.Quantity,
                    TotalPrice=it.Product.Price*it.Quantity
                });
                var product = db.Products.Find(it.ProductId);
                if (product != null)
                {
                    product.Quantity -= it.Quantity;
                    db.Entry(product).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            //send mail cho khachs hang
            var strSanPham = "";
            double thanhtien=0 ;
            double TongTien=0 ;
            foreach (var sp in cartItems)
            {
                strSanPham += "<tr>";
                strSanPham += "<td>" + sp.Product.Name + "</td>";
                strSanPham += "<td>" + sp.Quantity + "</td>";
                strSanPham += "<td>" + KQStore.Common.Common.FormatNumber(sp.Product.Price*sp.Quantity, 0) + "</td>";
                strSanPham += "</tr>";
                thanhtien += sp.Product.Price * sp.Quantity;
            }
            TongTien = thanhtien;
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
            contentCustomer = contentCustomer.Replace("{{MaDon}}", order.OrderCode);
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", user.Name);
            contentCustomer = contentCustomer.Replace("{{Phone}}", user.Phone);
            contentCustomer = contentCustomer.Replace("{{Email}}", user.Email);
            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.ShipAddress);
            contentCustomer = contentCustomer.Replace("{{Note}}", order.Note);
            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", KQStore.Common.Common.FormatNumber(thanhtien, 0));
            contentCustomer = contentCustomer.Replace("{{TongTien}}", KQStore.Common.Common.FormatNumber(TongTien, 0));
            
            KQStore.Common.Common.SendMail("KeqingShop", "Đơn hàng #" + order.OrderCode, contentCustomer.ToString(), user.Email);

            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
            contentAdmin = contentAdmin.Replace("{{MaDon}}", order.OrderCode);
            contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
            contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
            contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", user.Name);
            contentAdmin = contentAdmin.Replace("{{Phone}}", user.Phone);
            contentAdmin = contentAdmin.Replace("{{Email}}", user.Email);
            contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.ShipAddress);
            contentAdmin = contentAdmin.Replace("{{Note}}", order.Note);
            contentAdmin = contentAdmin.Replace("{{ThanhTien}}", KQStore.Common.Common.FormatNumber(thanhtien, 0));
            contentAdmin = contentAdmin.Replace("{{TongTien}}", KQStore.Common.Common.FormatNumber(TongTien, 0));
            KQStore.Common.Common.SendMail("KeqingShop", "Đơn hàng mới #" + order.OrderCode, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
            // xóa sp trong cart
           
            if (cart != null)
            {
                var cartItem = db.CartItems.Where(c => c.CartId == cart.CartId).ToList();
                db.CartItems.RemoveRange(cartItem);
                //db.Carts.Remove(cart);
                db.SaveChanges();
            }
            return View(order);
        }
        public ActionResult DonMua()
        {
            
            var user = Session["user"] as User;
            
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var userCheck = db.Users.SingleOrDefault(u => u.UserId == user.UserId);
            var orderDetails = db.OrderDetails
                              .Where(od => od.Order.UserId == userCheck.UserId)
                              .Include(od => od.Product)
                              
                              .ToList();

            // Lấy thông tin sản phẩm từ OrderDetails
            var products = orderDetails.Select(od => od.Product).Distinct().ToList();
            //ViewBag.Products = products;
            return View(products);
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
