using KQStore.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class ProductController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();
        // GET: Product
        public ActionResult Index(int? page, string searchProduct, string sortOrder, int? CateId, double? minPrice, double? maxPrice)
        {
            ViewBag.CurrentPage = "Shop";

            // Tim Kiem San Pham
            IQueryable<Product> products = db.Products;
            ViewBag.search = searchProduct;
            if (!String.IsNullOrEmpty(searchProduct))
            {
                products = products.Where(b => b.Name.Contains(searchProduct));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(b => b.Name);
                    break;
                default:
                    products = products.OrderBy(b => b.Name);
                    break;
            }

            
            if (CateId != null)
            {
                products = products.Where(b => b.CateId == CateId);
            }

            
            if (minPrice != null)
            {
                products = products.Where(b => b.Price >= minPrice);
            }

            if (maxPrice != null)
            {
                products = products.Where(b => b.Price <= maxPrice);
            }

           
            var pageSize = 9;
            var pageIndex = page ?? 1;  

            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CateId = CateId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(products.ToPagedList(pageIndex, pageSize));
        }

        public ActionResult Search(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return View("Error");
            }
            var results = db.Products.Where(p => p.Name.Contains(input)).ToList();

            return View("Index", results);
        }
        public ActionResult Detail(int productId)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
            return View(product);
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