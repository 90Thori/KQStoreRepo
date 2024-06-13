using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using KQStore.Models;
using PagedList;

namespace KQStore.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();

        // GET: Admin/Products
        public ActionResult Index(int? page, string searchProduct, string sortOrder)
        {
            // Tim Kiem San Pham
            IQueryable<Product> Product = db.Products;
            ViewBag.search = searchProduct;
            if (!String.IsNullOrEmpty(searchProduct))
            {
                Product = db.Products.Where(b => b.Name.Contains(searchProduct));
            }
            Product = Product.OrderBy(x => x.ProductId);

            //Sắp xếp 
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "name_desc":
                    Product = Product.OrderByDescending(b => b.ProductId);
                    break;
                default:
                    Product = Product.OrderBy(b => b.ProductId);
                    break;
            }

            //   Phân Trang
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;


            return View(Product.ToPagedList(pageIndex, pageSize));
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CateId = new SelectList(db.Categories, "CateId", "CateName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Name,Price,Discount,CateId,Detail,Img,Quantity,Rate,Created")] Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                if (product.Created == null)
                {
                    product.Created = DateTime.Now;
                }
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                string fileP = "~/Image/" + fileName;
                file.SaveAs(filePath);

                product.Img = fileP;
                db.Products.Add(product);

                db.SaveChanges();

                // Redirect hoặc trả về thông báo thành công
                return RedirectToAction("Index", "Products");

            }

            ViewBag.CateId = new SelectList(db.Categories, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CateId = new SelectList(db.Categories, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Name,Price,Discount,CateId,Detail,Img,Quantity,Rate,Created")] Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    string fileP = "~/Image/" + fileName;
                    file.SaveAs(filePath);

                    product.Img = fileP;
                }
                else
                {
                    
                    var existingAD = db.Products.AsNoTracking().FirstOrDefault(x => x.Img == product.Img);
                    if (existingAD != null)
                    {
                        product.Img = existingAD.Img;
                    }
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CateId = new SelectList(db.Categories, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //public ActionResult Search(string input)
        //{
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        return View("Error");
        //    }
        //    var results = db.Products.Where(p => p.Name.Contains(input)).ToList();

        //    return View("Index", results);
        //}


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
