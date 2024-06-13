using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KQStore.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace KQStore.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        private KQStoreEntities db = new KQStoreEntities();
        public ActionResult ThongKe()
        {
           
            return View();
        }

        
        [HttpGet]
        public ActionResult TongHop(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.OrderId equals od.OrderId
                        join p in db.Products on od.ProductId equals p.ProductId
                        select new
                        {
                            OrderDate = o.OrderDate,
                            Quantity = od.Quantity,
                            Price = (decimal)od.TotalPrice,
                            OriginalPrice = (decimal)p.Price
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.OrderDate >= startDate);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.OrderDate < endDate);
            }

            var result = query.GroupBy(x => System.Data.Entity.DbFunctions.TruncateTime(x.OrderDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            }).ToList();

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public ActionResult GetProductSales(string fromDate, string toDate)
        //{
        //    var querys = from o in db.tb_Order
        //                 where o.Status != "5"
        //                 join od in db.tb_OrderDetail
        //                 on o.Id equals od.OrderId
        //                 join p in db.tb_Product
        //                 on od.ProductId equals p.Id
        //                 select new
        //                 {
        //                     CreatedDate = o.CreatedDate,
        //                 };
        //    var query = from o in db.tb_Order
        //                where o.Status != "5"
        //                join od in db.tb_OrderDetail on o.Id equals od.OrderId
        //                join p in db.tb_Product on od.ProductId equals p.Id
        //                group od by p.Title into g
        //                select new
        //                {

        //                    ProductName = g.Key,
        //                    QuantitySold = g.Sum(od => od.Quantity)
        //                };
        //    if (!string.IsNullOrEmpty(fromDate))
        //    {
        //        if (DateTime.TryParseExact(fromDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime startDate))
        //        {
        //            querys = querys.Where(x => x.CreatedDate >= startDate);
        //        }
        //        //DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
        //        //query = query.Where(x => x.CreatedDate >= startDate);
        //    }
        //    if (!string.IsNullOrEmpty(toDate))
        //    {
        //        if (DateTime.TryParseExact(toDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime endDate))
        //        {
        //            querys = querys.Where(x => x.CreatedDate < endDate);
        //        }
        //        //DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
        //        //query = query.Where(x => x.CreatedDate < endDate);
        //    }
        //    var result = query.ToList();
        //    return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        //}

    }
}
