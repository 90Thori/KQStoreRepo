using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KQStore.Models;

namespace KQStore.Controllers
{
    public class UserController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();

     
        public ActionResult Details()
        {

            var userId = (Session["user"]as User)?.UserId; 
            var user=db.Users.Where(x=>x.UserId == userId).FirstOrDefault();
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }
            
            return View(user);
        }

        
        public ActionResult Edit()
        {
            var userId = (Session["user"] as User)?.UserId;
            var user = db.Users.Where(x => x.UserId == userId).FirstOrDefault();
            if (userId == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,RoleId,Name,Email,Phone,Password,CreatedAt,UpdatedAt,UserName,Address")] User user)
        {
            if (ModelState.IsValid)
            {
                
                user.UpdatedAt=DateTime.Now;
                user.RoleId = 2;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details","User");
            }
            
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ConfirmPW(string Password)
        {
            var userpass = (Session["user"] as User)?.Password;
            if (Password==userpass)
            {
                return RedirectToAction("NewPassW");
            }
            
            return Json(new { success = false, errorMessage = "Mật khẩu không đúng" }, JsonRequestBehavior.AllowGet );
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
