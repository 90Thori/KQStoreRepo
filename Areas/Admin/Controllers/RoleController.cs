using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KQStore.Models;

namespace KQStore.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();
        // GET: Admin/Role
        public ActionResult Index()
        {
            
            return View();
        }
    }
}