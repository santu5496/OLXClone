using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment4.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly IOrderManagement _dbConn;

        public OrderManagementController(IOrderManagement conn)
        {
            _dbConn = conn;
        }

        public IActionResult OrderManagement()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddOrEditOrder(Orders order, List<BillOrderItems> billOrderItems)
        {
            try
            {
                if (order.orderId == 0)
                {
                    var addedOrder = _dbConn.AddOrder(order, billOrderItems);
                    return Json(addedOrder);
                }
                else
                {
                    var updatedOrder = _dbConn.UpdateOrder(order, billOrderItems);
                    return Json(updatedOrder);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult DeleteOrder(int id)
        {
            try
            {
                return Json(_dbConn.DeleteOrder(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return Json(_dbConn.GetOrdersByDateRange(startDate, endDate));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

       
        public IActionResult GetAllOrders()
        {
            try
            {
                return Json(_dbConn.GetAllOrders());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
