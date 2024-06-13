using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class ProductImageController : Controller
    {
        private KQStoreEntities db= new KQStoreEntities();
        // GET: ProductImage
        public ActionResult Index(int ProductId)
        {
            var img = db.ProductImages.Where(p => p.ProductId == ProductId);
            return PartialView(img);
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