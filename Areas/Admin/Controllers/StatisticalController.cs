using KQStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KQStore.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private KQStoreEntities db=new KQStoreEntities();
        // GET: Admin/Statistical
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }
        
        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails
                        on o.OrderId equals od.OrderId
                        join p in db.Products
                        on od.ProductId equals p.ProductId
                        select new
                        {
                            CreatedDate = o.OrderDate,
                            Quantity = od.Quantity,
                            Price = od.TotalPrice,
                            OriginalPrice = p.Price
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetProductSales(string fromDate, string toDate)
        {
            var querys = from o in db.Orders
                         
                         join od in db.OrderDetails
                         on o.OrderId equals od.OrderId
                         join p in db.Products
                         on od.ProductId equals p.ProductId
                         select new
                         {
                             CreatedDate = o.OrderDate,
                         };
            var query = from o in db.Orders
                       
                        join od in db.OrderDetails on o.OrderId equals od.OrderId
                        join p in db.Products on od.ProductId equals p.ProductId
                        group od by p.Name into g
                        select new
                        {

                            ProductName = g.Key,
                            QuantitySold = g.Sum(od => od.Quantity)
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                if (DateTime.TryParseExact(fromDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime startDate))
                {
                    querys = querys.Where(x => x.CreatedDate >= startDate);
                }
                //DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                //query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                if (DateTime.TryParseExact(toDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime endDate))
                {
                    querys = querys.Where(x => x.CreatedDate < endDate);
                }
                //DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                //query = query.Where(x => x.CreatedDate < endDate);
            }
            var result = query.ToList();
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}