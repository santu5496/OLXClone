using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment4.Controllers
{
    public class RawInventoryController : Controller
    {
        private readonly IInventoryMaterialLog _dbConn;

        public RawInventoryController(IInventoryMaterialLog conn)
        {
            _dbConn = conn;
        }

        public IActionResult RawInventory()
        {
            return View();
        }

        public IActionResult GetPurchasedItems(DateTime startDate,DateTime endDate,List<int> supplierId,string paymentStatus)
        {
            try
            {
                var logs = _dbConn.GetPurchaseRecords(startDate,endDate,supplierId,paymentStatus);
                return Json(logs);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult AddOrEditRawMaterialLog(RawMaterialLogs log,List<PurchaseDetails> purchaseItems)
        {
            try
            {
                if (log.logId == 0)
                {
                    var result = _dbConn.SavePurchaseData(log,purchaseItems);
                    return Json(result);
                }
                else
                {
                    var result = _dbConn.UpdatePurchaseData(log,purchaseItems);
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var result = _dbConn.DeletePurchaseData(id);
                return Json(result ? "Log deleted successfully" : "Log not found");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetAllRawItems()
        {
            try
            {
                return Json(_dbConn.GetRawMaterialItems());
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        public IActionResult GetPurchasedItemsById(int purchaseId)
        {
            try
            {
                return Json(_dbConn.GetPurchasedItemsById(purchaseId));
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
    }
}
