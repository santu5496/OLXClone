using DbOperation.Implementation;
using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment4.Controllers
{
    public class FinishedGoodsController : Controller
    {
        private readonly IFinishedGoodsService _dbConn;
        public FinishedGoodsController(IFinishedGoodsService conn)
        {
            _dbConn = conn;
        }
        public IActionResult FinishedGoods()
        {
            return View();
        }

        public IActionResult AddorEditFinishedGoods(BakedCookedLogs fnGoods, List<ItemsUsedForGoods> items)
        {
            try
            {
                if (fnGoods.logId == 0)
                {
                    var addedItem = _dbConn.AddFinishedGoods(fnGoods, items);
                    return Json(addedItem);
                }
                else
                {
                    var updatedFinishedGoods = _dbConn.UpdateFinishedGoods(fnGoods, items);
                    return Json(updatedFinishedGoods);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult DeleteFinishedGoods(int id)
        {
            try
            {
                return Json(_dbConn.DeleteFinishedGoods(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetFinishedGoods(DateTime sd,DateTime ed)
        {
            try
            {
                return Json(_dbConn.GetFinishedGoods(sd,ed));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetInventoryItemsQuantity(int id)
        {
            try
            {
                return Json(_dbConn.GetInventoryItemsQuantityById(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetItemsUsedforProduct(int id,int baseQuantity)
        {
            try
            {
                return Json(_dbConn.GetItemsUsedforItem(id,baseQuantity));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetFinishedGoodsItems()
        {
            try
            {
                return Json(_dbConn.GetFinishedGoodsItems());
            }
            catch (Exception ex)
            {

                return Json(ex.Message);

            }
        }

        public IActionResult GetIngredientsUsedByLogId(int logId)
        {
            try
            {
                return Json(_dbConn.GetIngredientsUsedByLogId(logId));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
