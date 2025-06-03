using DbOperation.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZXing;
using SkiaSharp;
using ZXing.Common;
using DbOperation.Models;
using Newtonsoft.Json;
using Azure.Core.Extensions;
namespace Assignment4.Controllers
{
    public class BillingController : Controller
    {
        private readonly IBillingService _dbConn;
        private readonly IConfigurationService _dbConfig;

        public BillingController(IBillingService conn, IConfigurationService dbConfig)
        {
            _dbConn = conn;
            _dbConfig = dbConfig;
        }

        public IActionResult Billing()
        {
            return View();
        }
        public IActionResult GenerateBarCode()
        {
            return View();
        }
        public IActionResult BillRecords()
        {
            return View();
        }
        public IActionResult GetBillingItemsByCategory(int catId, string search)
        {
            try
            {
                return Json(_dbConn.GetBillingItemsByCategory(catId, search));
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        public IActionResult GenerateBarcodes([FromBody] List<int> itemIds)
        {
            var barcodes = new List<object>();

            var ItemList = _dbConn.GetFinishedGoodsItems();
            foreach (var itemId in itemIds)
            {
                var item = ItemList.FirstOrDefault(x => x.itemId == itemId);
                if (item == null) continue;

                var barcodeWriter = new ZXing.SkiaSharp.BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = 70,
                        Width = 150
                    }
                };

                var barcodeBitmap = barcodeWriter.Write(itemId.ToString());

                using (var ms = new MemoryStream())
                {
                    barcodeBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
                    var base64String = Convert.ToBase64String(ms.ToArray());

                    barcodes.Add(new
                    {
                        itemId,
                        itemName = item.itemName,
                        itemQuatity = item.priceQuantity,
                        itemUnit = item.unit,
                        barcode = $"data:image/png;base64,{base64String}"
                    });
                }

            }

            return Json(barcodes);
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

        public IActionResult GetDetailsOnBarcodeScan(int itemId)
        {
            try
            {
                return Json(_dbConn.GetInventoryItemById(itemId));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetOwnerBranchDetails()
        {
            try
            {
                return Json(_dbConn.GetOwnerBranchDetails());
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetCustomerDetailsById(int id)
        {
            try
            {
                return Json(_dbConfig.GetCustomerDetailsWithUnpaidBills(id));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);

            }
        }

        public IActionResult SaveBill(Billing billData, string billItems,int orderId)
        {
            try
            {
                var Data = JsonConvert.DeserializeObject<BillOrderItems[]>(billItems);
                var result = _dbConn.SaveBillData(billData, Data.ToList(),orderId);
                return Json(result);
            }
            catch (Exception e)
            {
                return Json(false);
            }
        }
        public IActionResult UpdateBill(Billing billData, string billItems)
        {
            try
            {
                var Data = JsonConvert.DeserializeObject<BillOrderItems[]>(billItems);
                var result = _dbConn.UpdateBillData(billData, Data.ToList());
                return Json(result);
            }
            catch (Exception e)
            {
                return Json(false);
            }
        }

        public IActionResult GetBillRecords(DateTime? startDate, DateTime? endDate, string paymentStatus, List<int> customerIds)
        {
            try
            {
                return Json(_dbConn.GetBillRecords(startDate, endDate, paymentStatus, customerIds));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetBillItmeDetails(int billId)
        {
            try
            {
                return Json(_dbConn.GetBillItemsByBillId(billId));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetOrderDetailsByCustomer(int customerId)
        {
            try
            {
                return Json(_dbConn.fetchOrderDetailsByCustomer(customerId));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
