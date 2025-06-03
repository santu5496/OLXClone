using DbOperation.Implementation;
using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assignment4.Controllers
{

    public class ReportsController : Controller
    {
        private readonly IReportService _dbConn;

        public ReportsController(IReportService db)
        {
            _dbConn = db;
            // GetBillReport(DateTime.Now.AddDays(-365), DateTime.Now);
        }
        public IActionResult DailyReport()
        {
            ViewBag.TotalSales = JsonConvert.SerializeObject(_dbConn.GetTotalSales(DateTime.Now));
            ViewBag.TotalProduced = JsonConvert.SerializeObject(_dbConn.GetTotalProduced(DateTime.Now));
            ViewBag.TotalPurchased = JsonConvert.SerializeObject(_dbConn.GetTotalPurchased(DateTime.Now));

            return View();
        }

        public IActionResult PurchaseSoldReports()
        {
            return View();
        }
        public IActionResult InventoryStock()
        {
            return View();
        }
        public JsonResult GetPurchaseReports(DateTime startDate, DateTime endDate)
        {
            var data = _dbConn.GetPurchaseReports(startDate, endDate);
            return Json(new { data });
        }
        public async Task<IActionResult> GetBillReport(DateTime startDate, DateTime endDate)
        {
            var report = await _dbConn.GetBillReportAsync(startDate, endDate);
            return Json(report);
        }

        public IActionResult GetInventoryStock()
        {
            var data = _dbConn.GetInventoryReport();
            return Json(new { data });
        }

        public IActionResult GetFinishedGoodsDashboardData(DateTime startDate, DateTime endDate)
        {
            try
            {
                var data = _dbConn.GetCookedLogsChart(startDate, endDate);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public IActionResult GetCookedLogsDetials(DateTime date)
        {
            try
            {
                var data = _dbConn.GetCookedLogDetailsByDate(date);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        
        public IActionResult UpdateInventoryQuantities(string data)
        {
            try
            {
                var updateList = new List<(int id, decimal quantity)>();
                var JsonData = JsonConvert.DeserializeObject<dynamic[]>(data);
                foreach (var item in JsonData)
                {
                    int id = Convert.ToInt32(item.id);
                    decimal quantity = Convert.ToDecimal(item.quantity.ToString());
                    updateList.Add((id, quantity));
                }

                var result = _dbConn.UpdateInventoryQuantities(updateList);
                return Json(result);
            }
            catch
            {
                return Json(false);
            }
        }


    }
}
