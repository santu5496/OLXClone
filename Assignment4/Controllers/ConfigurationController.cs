using System.Diagnostics;
using Assignment4.Models;
using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;
namespace Assignment4.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationService _dbConn;
        public ConfigurationController(IConfigurationService conn)
        {
            _dbConn = conn;
        }

        public IActionResult Configuration()
        {
            return View();
        }

        public IActionResult AddOrEditInventoryItem(InventoryItems item)
        {
            try
            {
                if (item.itemId == 0)
                {
                    var addedItem = _dbConn.AddInventoryItem(item);
                    return Json(addedItem);
                }
                else
                {
                    var updatedInventoryItem = _dbConn.UpdateInventoryItem(item);
                    return Json(updatedInventoryItem);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetInventoryItems()
        {
            try
            {
                return Json(_dbConn.GetInventoryItems());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult DeleteInventoryItems(int id)
        {
            try
            {
                return Json(_dbConn.DeleteInventoryItem(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult AddOrEditSupplierDetails(Suppliers supplier)
        {
            try
            {
                if (supplier.supplierId == 0)
                {
                    var addedSupplier = _dbConn.AddSupplier(supplier);
                    return Json(addedSupplier);

                }
                else
                {
                    var updatedSupplier = _dbConn.UpdateSupplier(supplier);
                    return Json(updatedSupplier);
                }
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetSupplierDts()
        {
            try
            {
                return Json(_dbConn.GetSupplierDts());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult DeleteSupplier(int id)
        {
            try
            {
                return Json(_dbConn.DeleteSupplier(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult AddorEditCategory(Category cat)
        {
            try
            {
                if (cat.id == 0)
                {
                    var addedCategory = _dbConn.AddCategory(cat);
                    return Json(addedCategory);

                }
                else
                {
                    var updatedCategory = _dbConn.UpdateCategory(cat);
                    return Json(updatedCategory);
                }
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public IActionResult GetCategory()
        {
            try
            {
                return Json(_dbConn.GetCategory());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public IActionResult DeleteCategory(int id)
        {
            try
            {
                return Json(_dbConn.DeleteCategory(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult AddorEditCustomer(Customers cust)
        {
            try
            {
                if (cust.customerId == 0)
                {
                    var addedCustomer = _dbConn.AddCustomer(cust);
                    return Json(addedCustomer);

                }
                else
                {
                    var updatedCustomer = _dbConn.UpdateCustomer(cust);
                    return Json(updatedCustomer);
                }
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetCustomer()
        {
            try
            {
                return Json(_dbConn.GetCustomers());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult DeleteCustomer(int id)
        {
            try
            {
                return Json(_dbConn.DeleteCustomer(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
