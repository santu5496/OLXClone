using DbOperation.Interface;
using Microsoft.AspNetCore.Mvc;
using DbOperation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using DbOperation.Implementation;

namespace Assignment4.Controllers
{
    public class ReturnManagementController : Controller
    {
        private readonly IReturnmanagementService _returnService;

        public ReturnManagementController(IReturnmanagementService returnService)
        {
            _returnService = returnService;
        }

        public IActionResult ReturnManagement()
        {
            return View();
        }


        public IActionResult GetInventoryItems()
        {
            try
            {
                return Json(_returnService.GetInventoryItems());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public bool AddOrEditReturn(ReturnManagement returnData, List<ReturnItems> returnItems)
        {
            try
            {
                if (returnData.returnId == null || returnData.returnId == 0)
                {
                   
                    bool result = _returnService.AddReturn(returnData, returnItems);
                    return result;
                }
                else
                {
                    
                    bool result = _returnService.UpdateReturn(returnData,returnItems);
                    return result;
                }
            }
            catch
            {
                return false;
            }
        }



      

        
        public bool DeleteReturn(int returnId)
        {
            try
            {
                bool result = _returnService.DeleteReturn(returnId);
                return result;
            }
            catch
            {
                return false;
            }
        }

       
        public IActionResult GetReturns()
        {
            try
            {
               
                var returns = _returnService.GetReturns();
                return Json(returns);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

     
        public IActionResult GetReturnItemsById(int returnId)
        {
            try
            {
                var returnData = _returnService.GetReturnItemsWithItemNameWithID(returnId);
                return Json(returnData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
