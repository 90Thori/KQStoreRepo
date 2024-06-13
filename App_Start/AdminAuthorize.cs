using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KQStore.App_Start
{
    public class AdminAuthorize : AuthorizeAttribute
    {
        public string RoleId { get; set; }

        //1check dang nhap
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            User nvSession = (User)HttpContext.Current.Session["user"];
            if (nvSession != null)
            {
                //2 check quyen: true => thuc hien filter 
                //nguoc lai tro lai trang Error
                KQStoreEntities db = new KQStoreEntities();

                var roles = RoleId.Split(',').Select(int.Parse);
                var count = db.Users.Count(m => m.UserId == nvSession.UserId && roles.Contains(m.RoleId));
                if (count != 0)
                {
                    return;
                }
                else
                {
                    var returnurl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    filterContext.Result = new RedirectToRouteResult(new
                      RouteValueDictionary(new { Controller = "Error", action = "KhongCoQuyen", area = "Admin", returnurl = returnurl.ToString() }));

                }
                return;
            }
            else
            {
                var returnurl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(new
                  RouteValueDictionary(new { Controller = "Homes", action = "Index", area = "Admin", returnurl = returnurl.ToString() }));
            }
        }
    }
}