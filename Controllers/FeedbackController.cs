using KQStore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class FeedbackController : Controller
    {
        // GET: Feedback
        private KQStoreEntities db= new KQStoreEntities();
        public ActionResult Index(int? productId)
        {
            
            var user = Session["user"] as User;
            var list = db.Feedbacks.Where(x => x.ProductId == productId).ToList();
            ViewBag.ProductId = productId;
            if (user == null)
            {
                ViewBag.Error = "Bạn cần đăng nhập để bình luận";
                ViewBag.HasPurchased = false;
            }
            else
            {
                var userCheck = db.Users.Where(u => u.UserId == user.UserId);

                var product = db.Products.Find(productId);
                var hasPurchased = db.Orders
                    .Join(db.OrderDetails,
                    order => order.OrderId,
                    orderDetail => orderDetail.OrderId,
                    (order, orderDetail) => new { Order = order, OrderDetail = orderDetail })
                    .Any(o => o.Order.UserId == user.UserId && o.OrderDetail.ProductId == productId);
                ViewBag.HasPurchased = hasPurchased;
            }
            var feedbackCount = db.Feedbacks.Count(f => f.ProductId == productId);
            ViewBag.FeedbackCount = feedbackCount;
            return PartialView(list);
        }

        [HttpPost]
        public ActionResult AddFb([Bind(Include = "FbId,ProductId,Subject,Content,FbImg,UserId")] Feedback fb, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                var user = Session["user"] as User;
                fb.UserId = user.UserId;
                fb.CreatedAt = DateTime.Now;

                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                string fileP = "~/Image/" + fileName;
                file.SaveAs(filePath);

                fb.FbImg = fileP;
                db.Feedbacks.Add(fb);
                db.SaveChanges();

                return RedirectToAction("Detail", "Product", new { ProductId = fb.ProductId });
            }
            return RedirectToAction("Detail", "Product", new { ProductId = fb.ProductId });
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