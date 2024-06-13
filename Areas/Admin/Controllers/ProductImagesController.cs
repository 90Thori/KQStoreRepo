using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KQStore.Models;

namespace KQStore.Areas.Admin.Controllers
{
    public class ProductImagesController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();

        // GET: Admin/ProductImages
        public ActionResult Index()
        {
            var productImages = db.ProductImages.Include(p => p.Product);
            return View(productImages.ToList());
        }

        // GET: Admin/ProductImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // GET: Admin/ProductImages/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name");
            return View();
        }

        // POST: Admin/ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ImgPath,ProductId")] ProductImage productImage, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                string fileP = "~/Image/" + fileName;
                file.SaveAs(filePath);

                productImage.ImgPath = fileP;
                db.ProductImages.Add(productImage);

                db.SaveChanges();

                // Redirect hoặc trả về thông báo thành công
                return RedirectToAction("Index", "ProductImages");

            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: Admin/ProductImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }

        // POST: Admin/ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ImgPath,ProductId")] ProductImage productImage,HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    string fileP = "~/Image/" + fileName;
                    file.SaveAs(filePath);

                    productImage.ImgPath = fileP;
                }
                else
                {
                    // Giữ nguyên giá trị AdsImage hiện tại
                    var existingAD = db.ProductImages.AsNoTracking().FirstOrDefault(x => x.ImgPath == productImage.ImgPath);
                    if (existingAD != null)
                    {
                        productImage.ImgPath = existingAD.ImgPath;
                    }
                }

                db.Entry(productImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: Admin/ProductImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: Admin/ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductImage productImage = db.ProductImages.Find(id);
            db.ProductImages.Remove(productImage);
            db.SaveChanges();
            return RedirectToAction("Index");
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
