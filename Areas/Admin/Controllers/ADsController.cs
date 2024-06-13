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
    public class ADsController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();

        // GET: Admin/ADs
        public ActionResult Index()
        {
            return View(db.ADS.ToList());
        }

        // GET: Admin/ADs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AD aD = db.ADS.Find(id);
            if (aD == null)
            {
                return HttpNotFound();
            }
            return View(aD);
        }

        // GET: Admin/ADs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ADs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdsId,AdsName,AdsDetail,AdsImage")] AD aD, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {

                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                string fileP = "~/Image/" + fileName;
                file.SaveAs(filePath);

                aD.AdsImage = fileP;
                db.ADS.Add(aD);

                db.SaveChanges();

                // Redirect hoặc trả về thông báo thành công
                return RedirectToAction("Index", "ADs");

            }


            return View(aD);
        }

        // GET: Admin/ADs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AD aD = db.ADS.Find(id);
            if (aD == null)
            {
                return HttpNotFound();
            }
            return View(aD);
        }

        // POST: Admin/ADs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdsId,AdsName,AdsDetail,AdsImage")] AD aD, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    string fileP = "~/Image/" + fileName;
                    file.SaveAs(filePath);

                    aD.AdsImage = fileP;
                }
                else
                {
                    // Giữ nguyên giá trị AdsImage hiện tại
                    var existingAD = db.ADS.AsNoTracking().FirstOrDefault(x => x.AdsId == aD.AdsId);
                    if (existingAD != null)
                    {
                        aD.AdsImage = existingAD.AdsImage;
                    }
                }

                db.Entry(aD).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aD);
        }

        // GET: Admin/ADs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AD aD = db.ADS.Find(id);
            if (aD == null)
            {
                return HttpNotFound();
            }
            return View(aD);
        }

        // POST: Admin/ADs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AD aD = db.ADS.Find(id);
            db.ADS.Remove(aD);
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
