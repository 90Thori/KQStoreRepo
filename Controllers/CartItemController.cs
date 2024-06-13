using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class CartItemController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();
        // GET: Cart
        public ActionResult Index()
        {
            var user = Session["user"] as User;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }        
            var cart = db.Carts.SingleOrDefault(c => c.UserId == user.UserId);        
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.UserId,
                    CreatedAt=DateTime.Now,
                };
                db.Carts.Add(cart);
                db.SaveChanges(); 
            }
           
            var cartItems = db.CartItems.Where(c => c.CartId == cart.CartId).Include(c => c.Product).ToList();

            var viewModel = new CheckoutViewModel
            {
                User = user,
                CartItems = cartItems
            };

            return View(cartItems);
        }
        public ActionResult ShowCount()
        {
            var userId = (Session["user"] as User)?.UserId;
            if (Session["user"] != null)
            {
                var cart = db.Carts.FirstOrDefault(c => c.UserId == userId.Value);
                if (cart != null)
                {
                    return Json(new { Count = db.CartItems.Count(x => x.CartId == cart.CartId) }, JsonRequestBehavior.AllowGet);
                }
            }
            
            return Json(new { Count =0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity )
        {
            var product = db.Products.Find(productId);
            
            if (product != null )
            {
                if (Session["user"] == null)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập trước khi thêm giỏ hàng!" });
                }

                if (product.Quantity < quantity)
                {
                    return Json(new { success = false, message = "Sản phẩm không đủ số lượng." });
                }
                var userId = (Session["user"] as User)?.UserId;
                if (userId != null)
                {
                    var cart = db.Carts.SingleOrDefault(c => c.UserId == userId.Value);
                    if (cart == null)
                    {
                        cart = new Cart { UserId = userId.Value, CreatedAt = DateTime.Now };
                        db.Carts.Add(cart);
                    }
                    var cartItem = cart.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
                    if (cartItem == null)
                    {
                        cartItem = new CartItem
                        {
                            CartId = cart.CartId,
                            ProductId = productId,
                            Quantity = quantity
                        };
                        db.CartItems.Add(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity += quantity;
                    }
                    var Count = db.CartItems.Count(x => x.CartId == cart.CartId);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Product added to cart." , Count });
                }
                else
                {
                    // Handle case for non-logged in users (use session or redirect to login)
                    // This part can be extended as per requirement
                    return Json(new { success = false, message = "User not logged in." });
                }
            }
            
            return Json(new { success = false, message = "Product not found." });
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            var userId = (Session["user"] as User)?.UserId;
            if (userId != null)
            {
                var cart = db.Carts.SingleOrDefault(c => c.UserId == userId.Value);
                if (cart != null)
                {
                    var cartItem = cart.CartItems.SingleOrDefault(ci => ci.ProductId == productId);
                    if (cartItem != null)
                    {
                        db.CartItems.Remove(cartItem);
                        db.SaveChanges();
                        return Json(new { success = true, message = "Product removed from cart." });
                    }
                }
                return Json(new { success = false, message = "Cart item not found." });
            }
            else
            {
                // Handle case for non-logged in users (use session or redirect to login)
                // This part can be extended as per requirement
                return Json(new { success = false, message = "User not logged in." });
            }
        }

        private Cart GetCart()
        {
            var userId = (Session["user"] as User)?.UserId;
            if (userId != null)
            {
                var cart = db.Carts.Include("CartItems").SingleOrDefault(c => c.UserId == userId.Value);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId.Value, CreatedAt = DateTime.Now };
                    db.Carts.Add(cart);
                    db.SaveChanges();
                }
                return cart;
            }
            return null;
        }

        [HttpPost]
        public JsonResult UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            var user = Session["user"] as User;
            if (user == null)
            {
                return Json(new { success = false, message = "Please log in to update cart item quantity." });
            }

            var cartItem = db.CartItems.SingleOrDefault(c => c.CartItemId == cartItemId && c.Cart.UserId == user.UserId);
            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                db.SaveChanges();

                // Tính lại tổng giá của sản phẩm này
                var totalPrice = cartItem.Product.Price * cartItem.Quantity;

                // Tính tổng đơn hàng
                var cartTotal = db.CartItems.Where(c => c.Cart.UserId == user.UserId)
                                            .Sum(c => c.Product.Price * c.Quantity);
                var cartTotalSummary = cartTotal;
                return Json(new { success = true, message = "Cart item quantity updated.", totalPrice = totalPrice, cartTotal = cartTotal });
            }

            return Json(new { success = false, message = "Cart item not found." });
        }
        public ActionResult CountCart(int cartid)
        {
            int count = db.CartItems.Where(p => p.CartId == 1).Count();
            return View();
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