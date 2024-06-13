using System.Web.Mvc;

namespace KQStore.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Homes", action = "LoginAdmin", id = UrlParameter.Optional },
                namespaces: new[] { "KQStore.Areas.Admin.Controllers" }
            );

        }
    }
}