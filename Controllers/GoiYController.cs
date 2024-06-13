using KQStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class GoiYController : Controller
    {
        // GET: GoiY
        private readonly DatabaseHandler dbHandler;
        private readonly ProductRecommender recommender;
        
        // Nếu bạn sử dụng Dependency Injection
        public GoiYController()
        {
            dbHandler = new DatabaseHandler();
            recommender = new ProductRecommender();
        }

        public ActionResult Index(int productId)
        {
            try
            {
                DataTable productsTable = dbHandler.GetProducts();
                recommender.LoadProducts(productsTable);

                int productIndex = recommender.Products.FindIndex(p => p.ProductId == productId); // Tìm chỉ số của sản phẩm theo productId
                int numberOfSimilarProducts = 5;

                var topSimilarProducts = recommender.GetTopSimilarProducts(productIndex, numberOfSimilarProducts);

                ViewBag.TopSimilarProducts = topSimilarProducts;
                return PartialView("_ViewGoiY");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ở đây
                ViewBag.Error = ex.Message;
                return View("Error");
            }
            finally
            {
                // Optional: If you need to clear resources manually
                recommender.Products.Clear();
            }
        }
        
    }
}