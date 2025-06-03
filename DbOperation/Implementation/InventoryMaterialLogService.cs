using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using DbOperation.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using DbOperation.ViewModels;

namespace DbOperation.Implementation
{
    public class InventoryMaterialLogService : IInventoryMaterialLog
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public InventoryMaterialLogService(string context)
        {
           
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(context).Options;
           
        }

        #region RawMaterialLogsCRUD
        //public List<RawMaterialLogViewModel> GetRawMaterialLogsWithSupplierAndItem()
        //{
        //    using var dbConn = new Assignment4Context(_dbConn);

        //    var result = dbConn.RawMaterialLogs
        //        .Include(r => r.supplier)
        //        .Include(r => r.item)
        //        .Select(r => new RawMaterialLogViewModel
        //        {
        //            LogId = r.logId,
        //            SupplierName = r.supplier.supplierName,
        //            ItemName = r.item.itemName, 
        //            Quantity = r.quantity,
        //            CreatedDate = r.createdDate,
        //            UpdatedDate=r.updatedDate,
        //            User=r.sUser,
        //            itemID=r.item.itemId,
        //            supplierId=r.supplier.supplierId


        //        })
        //        .ToList();

        //    return result;
        //}

        public bool SavePurchaseData(RawMaterialLogs purchaseData, List<PurchaseDetails> items)
        {
            try
            {
                var db = new Assignment4Context(_dbConn);

                purchaseData.purchaseDate = DateTime.Now;
                purchaseData.createdDate = DateTime.Now;    
                purchaseData.updatedDate = DateTime.Now;
                purchaseData.sUser = "Admin";
                db.Add(purchaseData);
                db.SaveChanges();

                // Process Purchase Details Items
                foreach (var item in items)
                {
                    var purchaseItem = new PurchaseDetails
                    {
                        fkRawMaterialId = purchaseData.logId,
                        fkItemId = item.fkItemId,
                        quantity = item.quantity,
                        unit = item.unit,
                        pricePerUnit = item.pricePerUnit,
                        totalAmount = item.quantity * item.pricePerUnit,
                    };
                    db.Add(purchaseItem);

                    // Add to Inventory
                    var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == item.fkItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity += item.quantity;
                        inventoryItem.updatedDate = DateTime.Now;
                        db.Update(inventoryItem);
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdatePurchaseData(RawMaterialLogs purchaseData, List<PurchaseDetails> items)
        {
            try
            {
                var db = new Assignment4Context(_dbConn);

                // Update Raw Material Log Data
                var existingPurchase = db.RawMaterialLogs.FirstOrDefault(p => p.logId == purchaseData.logId);
                if (existingPurchase != null)
                {
                    existingPurchase.supplierId = purchaseData.supplierId;
                    existingPurchase.totalAmount = purchaseData.totalAmount;
                    existingPurchase.advanceAmount = purchaseData.advanceAmount;
                    existingPurchase.paymentMode = purchaseData.paymentMode;
                    existingPurchase.paymentStatus = purchaseData.paymentStatus;
                    existingPurchase.purchaseDate = DateTime.Now;
                    existingPurchase.updatedDate = DateTime.Now;
                    existingPurchase.sUser = "Admin";
                    db.Update(existingPurchase);
                }

                // Restore Inventory Before Removing Old Items
                var existingItems = db.PurchaseDetails.Where(p => p.fkRawMaterialId == purchaseData.logId).ToList();
                foreach (var existingItem in existingItems)
                {
                    var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == existingItem.fkItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity -= existingItem.quantity;
                        inventoryItem.updatedDate = DateTime.Now;
                        db.Update(inventoryItem);
                    }
                    db.PurchaseDetails.Remove(existingItem);
                }

                // Process New Purchase Details Items
                foreach (var item in items)
                {
                    var newItem = new PurchaseDetails
                    {
                        fkRawMaterialId = purchaseData.logId,
                        fkItemId = item.fkItemId,
                        quantity = item.quantity,
                        unit = item.unit,
                        pricePerUnit = item.pricePerUnit,
                        totalAmount = item.quantity * item.pricePerUnit,
                    };
                    db.Add(newItem);

                    var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == item.fkItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity += item.quantity;
                        inventoryItem.updatedDate = DateTime.Now;
                        db.Update(inventoryItem);
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public bool DeletePurchaseData(int logId)
        {
            try
            {
                var db = new Assignment4Context(_dbConn);

                // Find the purchase log
                var existingLog = db.RawMaterialLogs.FirstOrDefault(r => r.logId == logId);
                if (existingLog == null)
                {
                    return false;
                }

                // Find and restore inventory for related purchase details
                var purchaseDetails = db.PurchaseDetails.Where(p => p.fkRawMaterialId == logId).ToList();
                foreach (var detail in purchaseDetails)
                {
                    var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == detail.fkItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity -= detail.quantity;
                        inventoryItem.updatedDate = DateTime.Now;
                        db.Update(inventoryItem);
                    }

                    db.PurchaseDetails.Remove(detail); // Remove the purchase detail
                }

                // Remove the raw material log itself
                db.RawMaterialLogs.Remove(existingLog);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public List<dynamic> GetRawMaterialLogs()
        {
            try
            {
                using var db = new Assignment4Context(_dbConn);

                var rawMaterialLogs = (from log in db.RawMaterialLogs
                                       join supplier in db.Suppliers
                                       on log.supplierId equals supplier.supplierId
                                       select new
                                       {
                                           logId = log.logId,
                                           supplierName = supplier.supplierName,
                                           totalAmount = log.totalAmount,
                                           paymentStatus = log.paymentStatus,
                                           paymentMode = log.paymentMode,
                                           purchaseDate = log.purchaseDate
                                       }).ToList<dynamic>();

                return rawMaterialLogs;
            }
            catch (Exception ex)
            {
                return new List<dynamic>();
            }
        }

        //public RawMaterialLogs? GetRawMaterialLogById(int logId)
        //{
        //    try
        //    {
        //        using var dbConn = new Assignment4Context(_dbConn);
        //        return dbConn.RawMaterialLogs.Include(r => r.item).Include(r => r.supplier)
        //                                       .FirstOrDefault(r => r.logId == logId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in GetRawMaterialLogById: {ex.Message}");
        //        return null;
        //    }
        //}

        public bool UpdateRawMaterialLog(List<RawMaterialLogs> log)
        {
            throw new NotImplementedException();
        }

        public List<InventoryItems> GetRawMaterialItems()
        {
            var db = new Assignment4Context(_dbConn);
            return db.InventoryItems.Where(x=>x.itemType=="raw").ToList();
        }
        public List<PurchaseViewModel> GetPurchaseRecords(DateTime? startDate, DateTime? endDate, List<int> supplierIds, string paymentStatus)
        {
            using var db = new Assignment4Context(_dbConn);

            var query = from purchase in db.RawMaterialLogs
                        join supplier in db.Suppliers
                        on purchase.supplierId equals supplier.supplierId into suppGroup
                        from supp in suppGroup.DefaultIfEmpty()
                        select new PurchaseViewModel
                        {
                            purchaseId = purchase.logId,
                            fkSupplierId = purchase.supplierId,
                            purchaseDate = purchase.purchaseDate,
                            totalAmount = purchase.totalAmount,
                            paymentStatus = purchase.paymentStatus,
                            paymentMode = purchase.paymentMode,
                            createdDate = purchase.createdDate,
                            sUser = purchase.sUser,
                            supplierName = supp != null ? supp.supplierName : string.Empty
                        };

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(p => p.purchaseDate >= startDate.Value && p.purchaseDate <= endDate.Value);
            }

            if (supplierIds != null && supplierIds.Any())
            {
                query = query.Where(p => supplierIds.Contains(p.fkSupplierId ?? 0));
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query = query.Where(p => p.paymentStatus == paymentStatus);
            }

            return query.ToList();
        }

        public List<dynamic> GetPurchasedItemsById(int purchaseId)
        {
            var dbContext = new Assignment4Context(_dbConn);

            var purchasedItems = (from purchaseDetail in dbContext.PurchaseDetails
                                  join inventoryItem in dbContext.InventoryItems on purchaseDetail.fkItemId equals inventoryItem.itemId
                                  where purchaseDetail.fkRawMaterialId == purchaseId
                                  select new
                                  {
                                      purchaseDetailId = purchaseDetail.purchaseDetailId,
                                      rawMaterialId = purchaseDetail.fkRawMaterialId,
                                      itemId = purchaseDetail.fkItemId,
                                      quantity = purchaseDetail.quantity,
                                      unit = purchaseDetail.unit,
                                      pricePerUnit = purchaseDetail.pricePerUnit,
                                      totalAmount = purchaseDetail.totalAmount,
                                      itemName = inventoryItem.itemName
                                  }).ToList();

            return purchasedItems.Cast<dynamic>().ToList();
        }


        #endregion
    }
}
