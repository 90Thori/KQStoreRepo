using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class CategoryController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();
        // GET: Category
        public ActionResult Index()
        {
                
            var category = db.Categories.ToList();
            return PartialView("_Index", category);
        }
        public ActionResult List()
        {
            var categories = db.Categories.ToList();
            var categoryRandomImages = new List<CategoryRandomImage>();

            foreach (var category in categories)
            {
                var randomProduct = category.Products.OrderBy(x => Guid.NewGuid()).FirstOrDefault(); // Lấy một sản phẩm ngẫu nhiên
                var randomImage = randomProduct != null ? randomProduct.Img : "~/Image/default.jpg"; // Nếu sản phẩm có ảnh, lấy ảnh của sản phẩm, ngược lại lấy ảnh mặc định

                categoryRandomImages.Add(new CategoryRandomImage
                {
                    Category = category,
                    RandomImageUrl = randomImage
                });
            }

            return PartialView("_List",categoryRandomImages);
        }
        public ActionResult ListLayout()
        {
            var listLO=db.Categories.ToList();
            return PartialView("_ListLayout", listLO);
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
//public class CategoryRandomImage
//{
//    public Category Category { get; set; }
//    public string RandomImageUrl { get; set; }
//}