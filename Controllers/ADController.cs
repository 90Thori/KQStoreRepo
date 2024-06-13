using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Controllers
{
    public class ADController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();
        // GET: AD
        public ActionResult Index()
        {
            var ADs = db.ADS.ToList();
            return PartialView("_Index", ADs);
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