using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Reflection.Metadata;
using ZXing.OneD;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
namespace DbOperation.Implementation
{
    public class ReportService : IReportService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;
        private readonly string _connectionString;
        public ReportService(string dbConn, string connectionString)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(dbConn).Options;
            _connectionString = connectionString;
        }
        public List<dynamic> GetTotalSales(DateTime reportDate)
        {
            var db = new Assignment4Context(_dbConn);

            var result = (from boi in db.BillOrderItems
                          join inv in db.Inventory on boi.fkItemId equals inv.itemID
                          join iItem in db.InventoryItems on boi.fkItemId equals iItem.itemId
                          join b in db.Billing on boi.fkBillOrderId equals b.billId
                          where b.billDate.Date == reportDate.Date
                          group boi by new { iItem.itemName, iItem.unit } into sales
                          select new
                          {
                              ItemName = sales.Key.itemName,
                              TotalSold = sales.Sum(x => x.quantity) + sales.Key.unit
                          }).ToList<dynamic>();

            return result;
        }


        public List<dynamic> GetTotalProduced(DateTime reportDate)
        {
            var db = new Assignment4Context(_dbConn);
            var result = (from bl in db.BakedCookedLogs
                    join inv in db.Inventory on bl.fkItemId equals inv.itemID
                    join iItem in db.InventoryItems on bl.fkItemId equals iItem.itemId
                    where bl.createdDate.Date == reportDate.Date
                    group bl by new { iItem.itemName, iItem.unit } into produced
                    select new
                    {
                        ItemName = produced.Key.ToString(),
                        TotalProduced = produced.Sum(x => x.actualQuantity) + produced.Key.unit
                    }).ToList<dynamic>();

            return result;
        }

        public List<dynamic> GetTotalPurchased(DateTime reportDate)
        {
            var db = new Assignment4Context(_dbConn);
            var result = (from pd in db.PurchaseDetails
                          join rml in db.RawMaterialLogs on pd.fkRawMaterialId equals rml.logId
                          join iItem in db.InventoryItems on pd.fkItemId equals iItem.itemId
                          where rml.createdDate.Date == reportDate.Date
                          group pd by new { iItem.itemName, iItem.unit } into purchased
                          select new
                          {
                              itemName = purchased.Key.ToString(),
                              totalPurchased = purchased.Sum(x => x.quantity) + purchased.Key.unit
                          }).ToList<dynamic>();
            return result;
        }
        public List<dynamic> GetPurchaseReports(DateTime startDate, DateTime endDate)
        {
            using (var db = new Assignment4Context(_dbConn))
            {
                var result = (from log in db.RawMaterialLogs
                              join detail in db.PurchaseDetails on log.logId equals detail.fkRawMaterialId
                              join supplier in db.Suppliers on log.supplierId equals supplier.supplierId
                              join item in db.InventoryItems on detail.fkItemId equals item.itemId
                              where log.purchaseDate >= startDate && log.purchaseDate <= endDate
                              select new
                              {
                                  logId = log.logId,
                                  supplierName = supplier.supplierName,
                                  purchaseDate = log.purchaseDate,
                                  totalAmount = log.totalAmount,
                                  advanceAmount = log.advanceAmount,
                                  paymentMode = log.paymentMode,
                                  paymentStatus = log.paymentStatus,
                                  itemName = item.itemName,
                                  quantity = detail.quantity,
                                  unit = detail.unit,
                                  quantityUnit = detail.quantity + " " + detail.unit,  // Combined Quantity & Unit
                                  pricePerUnit = detail.pricePerUnit,
                                  detailTotalAmount = detail.totalAmount
                              }).ToList<dynamic>();

                return result;
            }
        }


        public async Task<List<dynamic>> GetBillReportAsync(DateTime startDate, DateTime endDate)
        {
            var db = new Assignment4Context(_dbConn);
            var billReport = await (from bill in db.Billing
                                    join customer in db.Customers on bill.fkCustomerId equals customer.customerId
                                    join billOrderItem in db.BillOrderItems on bill.billId equals billOrderItem.fkBillOrderId
                                    join item in db.InventoryItems on billOrderItem.fkItemId equals item.itemId
                                    where bill.billDate >= startDate && bill.billDate <= endDate
                                          && billOrderItem.entryType == "bill"
                                    select new
                                    {
                                        CustomerName = customer.customerName,
                                        BillDate = bill.billDate,
                                        PaymentMode = bill.paymentMode,
                                        PaymentStatus = bill.paymentStatus,
                                        ItemName = item.itemName,
                                        Quantity = billOrderItem.quantity,
                                        TotalAmount = bill.finalAmount
                                    }).ToListAsync();

            return billReport.Cast<dynamic>().ToList();
        }
        //public byte[] GeneratePDFReport(DateTime reportDate,
        //                                List<dynamic> totalSales,
        //                                List<dynamic> totalProduced,
        //                                List<dynamic> totalPurchased)
        //{
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        Document document = new Document(PageSize.A4);
        //        PdfWriter.GetInstance(document, stream);
        //        document.Open();

        //        document.Add(new Paragraph($"Daily Report for {reportDate:yyyy-MM-dd}\n\n"));

        //        document.Add(new Paragraph("Total Sales Report:"));
        //        foreach (var item in totalSales)
        //        {
        //            document.Add(new Paragraph($"{item.ItemName} - {item.TotalSold} units"));
        //        }

        //        document.Add(new Paragraph("\nTotal Finished Goods Produced Report:"));
        //        foreach (var item in totalProduced)
        //        {
        //            document.Add(new Paragraph($"{item.ItemName} - {item.TotalProduced} units"));
        //        }

        //        document.Add(new Paragraph("\nTotal Items Purchased Report:"));
        //        foreach (var item in totalPurchased)
        //        {
        //            document.Add(new Paragraph($"{item.ItemName} - {item.TotalPurchased} units"));
        //        }

        //        document.Close();
        //        return stream.ToArray();
        //    }
        //}


        public List<dynamic> GetInventoryReport()
        {
            using (var db = new Assignment4Context(_dbConn))
            {
                var result = (from inv in db.Inventory
                              join item in db.InventoryItems on inv.itemID equals item.itemId
                              select new
                              {
                                  id= item.itemId,
                                  itemName = item.itemName,
                                  quantity = inv.quantity,
                                  unit = item.unit,
                                  lastUpdated = inv.updatedDate
                              }).ToList<dynamic>();
                return result;
            }
        }

        public bool UpdateInventoryQuantities(List<(int id, decimal quantity)> updateList)
        {
            try
            {
                var db = new Assignment4Context(_dbConn);

                foreach (var (id, quantity) in updateList)
                {
                    var inventoryItem = db.Inventory.FirstOrDefault(x => x.itemID == id);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity = quantity;
                        inventoryItem.updatedDate = DateTime.Now;
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        #region Dashboard Queries 

        public List<dynamic> GetCookedLogDetailsByDate(DateTime date)
        {
           
            var connection = new SqlConnection(_connectionString);
                string query = @"
                                SELECT 
                                    i.itemName AS ItemName,
                                    l.targetQuantity AS TargetQuantity,
                                    l.actualQuantity AS ActualQuantity,
                                    l.reasonForDiff AS ReasonForDiff,
                                    l.createdDate AS Date
                                FROM BakedCookedLogs l
                                INNER JOIN InventoryItems i ON l.fkItemId = i.itemId
                                WHERE l.createdDate >= @StartDate AND l.createdDate < @EndDate";

                var parameters = new
                {
                    StartDate = date.Date,
                    EndDate = date.Date.AddDays(1)
                };

                return connection.Query(query, parameters).ToList();
            
        }
        public List<dynamic> GetCookedLogsChart(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                        SELECT 
                            CAST(createdDate AS DATE) AS Date,
                            SUM(targetQuantity) AS TotalTarget,
                            SUM(actualQuantity) AS TotalActual
                        FROM BakedCookedLogs
                        WHERE createdDate >= @StartDate AND createdDate < @EndDate
                        GROUP BY CAST(createdDate AS DATE)
                        ORDER BY Date";

                var parameters = new
                {
                    StartDate = startDate.Date,
                    EndDate = endDate.Date.AddDays(1)
                };

                return connection.Query(query, parameters).ToList();
            }
        }


        #endregion
    }
}
