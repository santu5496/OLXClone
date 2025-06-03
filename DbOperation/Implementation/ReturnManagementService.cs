using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbOperation.Implementation
{
    public class ReturnManagementService : IReturnmanagementService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public ReturnManagementService(string context)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(context).Options;
        }

        #region AddMultipleReturns - IMPROVED
        public bool AddMultipleReturns(List<ReturnManagement> items)
        {
            using var Db = new Assignment4Context(_dbConn);
            using var transaction = Db.Database.BeginTransaction();

            try
            {
                Db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ReturnManagement] ON");

                if (items.Count > 0)
                {
                    // Add the first item to get the returnId
                    items[0].createdDate = DateTime.Now;
                    items[0].updatedDate = DateTime.Now;
                    Db.Add(items[0]);
                    Db.SaveChanges();

                    int returnId = items[0].returnId;

                    // Add remaining items with the same returnId
                    foreach (var item in items.Skip(1))
                    {
                        item.returnId = returnId;
                        item.createdDate = DateTime.Now;
                        item.updatedDate = DateTime.Now;
                        Db.Add(item);
                    }
                    Db.SaveChanges();
                }

                Db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ReturnManagement] OFF");
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in AddMultipleReturns: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region AddReturn - IMPROVED
        public bool AddReturn(ReturnManagement returnData, List<ReturnItems> returnItems)
        {
            using var Db = new Assignment4Context(_dbConn);
            using var transaction = Db.Database.BeginTransaction();

            try
            {
                // Set default values and timestamps
                returnData.sUser = "normaluser";
                returnData.itemId = 1;
                returnData.createdDate = DateTime.Now;
                returnData.updatedDate = DateTime.Now;
                returnData.returnDate = returnData.returnDate ?? DateTime.Now;

                Db.Add(returnData);
                Db.SaveChanges();

                // Set foreign key for return items
                foreach (var item in returnItems)
                {
                    item.fkReturnId = returnData.returnId;
                }

                // Add return items and update inventory
                AddReturnItemsInternal(returnItems, Db);

                Db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in AddReturn: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region AddReturnItems - IMPROVED
        public bool AddReturnItems(List<ReturnItems> returnItems)
        {
            using var Db = new Assignment4Context(_dbConn);
            using var transaction = Db.Database.BeginTransaction();

            try
            {
                AddReturnItemsInternal(returnItems, Db);
                Db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in AddReturnItems: {ex.Message}");
                return false;
            }
        }

        private void AddReturnItemsInternal(List<ReturnItems> returnItems, Assignment4Context Db)
        {
            foreach (var item in returnItems)
            {
                // Add the return item
                Db.Add(item);
                Db.SaveChanges(); // Need to save to get the ID

                // Update inventory if reuse destination is specified
                if (item.fkReuseDestianationItemId.HasValue && item.fkReuseDestianationItemId > 0)
                {
                    var inventoryItem = Db.Inventory.FirstOrDefault(i => i.itemID == item.fkReuseDestianationItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.updatedDate = DateTime.Now;
                        inventoryItem.sUser = "User";
                        inventoryItem.quantity += (decimal)(item.quantity ?? 0);
                        Db.Update(inventoryItem);
                    }
                    else
                    {
                        // Create new inventory record if it doesn't exist
                        var newInventoryItem = new Inventory
                        {
                            itemID = item.fkReuseDestianationItemId.Value,
                            quantity = (decimal)(item.quantity ?? 0),
                            createdDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            sUser = "User"
                        };
                        Db.Add(newInventoryItem);
                    }
                }
            }
        }
        #endregion

        #region UpdateReturn - COMPLETELY FIXED
     
        #endregion
        private void UpdateReturnItemsInternal(List<ReturnItems> returnItems, Assignment4Context Db)
{
    // Get the return ID from the first item (assuming all items belong to the same return)
    var returnId = returnItems.FirstOrDefault()?.fkReturnId;
    if (!returnId.HasValue)
    {
        throw new ArgumentException("No valid return ID found in return items");
    }

    // Get all existing items for this return from the database
    var existingItems = Db.ReturnItems.Where(r => r.fkReturnId == returnId.Value).ToList();

    foreach (var item in returnItems)
    {
        // Ensure the item has the correct return ID
        item.fkReturnId = returnId.Value;

        // Check if this item already exists in the database
        var existingItem = existingItems.FirstOrDefault(existing =>
            existing.fkInventoryItemId == item.fkInventoryItemId);

        if (existingItem != null)
        {
            // Update existing item
            UpdateExistingReturnItem(existingItem, item, Db);
        }
        else
        {
            // This is a new item - add it
            // Make sure all required fields are set
            if (item.returnItemTblId == 0)
            {
                item.returnItemTblId = 0; // Let EF handle the ID generation
            }
            
            AddNewReturnItem(item, Db);
        }
    }

    // Handle items that were removed (exist in DB but not in the update list)
    var itemsToRemove = existingItems.Where(existing =>
        !returnItems.Any(updated => updated.fkInventoryItemId == existing.fkInventoryItemId)).ToList();

    foreach (var itemToRemove in itemsToRemove)
    {
        RemoveReturnItem(itemToRemove, Db);
    }
}

private void AddNewReturnItem(ReturnItems item, Assignment4Context Db)
{
    // Ensure the item is properly configured for insertion
    item.returnItemTblId = 0; // Reset ID to let EF generate new one
    
    // Add the new item
    Db.Add(item);
    
    // Note: Don't save changes here - let the parent method handle it
    // This allows for better transaction management

    // Add quantity to inventory if reuse destination is specified
    if (item.fkReuseDestianationItemId.HasValue && item.fkReuseDestianationItemId > 0)
    {
        var inventoryItem = Db.Inventory.FirstOrDefault(i => i.itemID == item.fkReuseDestianationItemId);
        if (inventoryItem != null)
        {
            inventoryItem.quantity += (decimal)(item.quantity ?? 0);
            inventoryItem.updatedDate = DateTime.Now;
            inventoryItem.sUser = "User";
            Db.Update(inventoryItem);
        }
        else
        {
            var newInventoryItem = new Inventory
            {
                itemID = item.fkReuseDestianationItemId.Value,
                quantity = (decimal)(item.quantity ?? 0),
                createdDate = DateTime.Now,
                updatedDate = DateTime.Now,
                sUser = "User"
            };
            Db.Add(newInventoryItem);
        }
    }
}

// Also make sure your UpdateReturn method is handling the return items correctly
public bool UpdateReturn(ReturnManagement returnData, List<ReturnItems> returnItems)
{
    using var Db = new Assignment4Context(_dbConn);
    using var transaction = Db.Database.BeginTransaction();

    try
    {
        var existingReturn = Db.ReturnManagement.FirstOrDefault(r => r.returnId == returnData.returnId);
        if (existingReturn == null)
        {
            transaction.Rollback();
            return false;
        }

        // Update return management record
        existingReturn.itemId = returnData.itemId > 0 ? returnData.itemId : 1;
        existingReturn.returnDescription = returnData.returnDescription;
        existingReturn.updatedDate = DateTime.Now;
        existingReturn.TotalReturnPrice = returnData.TotalReturnPrice;
        existingReturn.sUser = "User";
        existingReturn.fkcotomerID = returnData.fkcotomerID;

        Db.Update(existingReturn);

        // Ensure all return items have the correct foreign key
        foreach (var item in returnItems)
        {
            item.fkReturnId = returnData.returnId;
        }

        // Update return items with proper inventory handling
        UpdateReturnItemsInternal(returnItems, Db);

        Db.SaveChanges();
        transaction.Commit();
        return true;
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        Console.WriteLine($"Error in UpdateReturn: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        return false;
    }
}
        #region UpdateReturns - COMPLETELY REWRITTEN
        public void UpdateReturns(List<ReturnItems> returnItems)
        {
            using var Db = new Assignment4Context(_dbConn);
            using var transaction = Db.Database.BeginTransaction();

            try
            {
                UpdateReturnItemsInternal(returnItems, Db);
                Db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in UpdateReturns: {ex.Message}");
                throw;
            }
        }

       

        private void UpdateExistingReturnItem(ReturnItems existingItem, ReturnItems newItem, Assignment4Context Db)
        {
            // Store old values for inventory adjustment
            var oldQuantity = existingItem.quantity ?? 0;
            var oldReuseDestination = existingItem.fkReuseDestianationItemId;

            var newQuantity = newItem.quantity ?? 0;
            var newReuseDestination = newItem.fkReuseDestianationItemId;

            // Update the return item properties
            existingItem.quantity = newItem.quantity;
            existingItem.unit = newItem.unit;
            existingItem.returnReason = newItem.returnReason;
            existingItem.returnDescription = newItem.returnDescription;
            existingItem.discount = newItem.discount;
            existingItem.returnPrice = newItem.returnPrice;
            existingItem.ReuseDestination = newItem.ReuseDestination;
            existingItem.fkReuseDestianationItemId = newItem.fkReuseDestianationItemId;

            Db.Update(existingItem);

            // Handle inventory adjustments
            if (oldReuseDestination.HasValue && oldReuseDestination > 0)
            {
                // Remove old quantity from old destination
                var oldInventoryItem = Db.Inventory.FirstOrDefault(i => i.itemID == oldReuseDestination);
                if (oldInventoryItem != null)
                {
                    oldInventoryItem.quantity -= (decimal)oldQuantity;
                    oldInventoryItem.updatedDate = DateTime.Now;
                    oldInventoryItem.sUser = "User";
                    Db.Update(oldInventoryItem);
                }
            }

            if (newReuseDestination.HasValue && newReuseDestination > 0)
            {
                // Add new quantity to new destination
                var newInventoryItem = Db.Inventory.FirstOrDefault(i => i.itemID == newReuseDestination);
                if (newInventoryItem != null)
                {
                    newInventoryItem.quantity += (decimal)newQuantity;
                    newInventoryItem.updatedDate = DateTime.Now;
                    newInventoryItem.sUser = "User";
                    Db.Update(newInventoryItem);
                }
                else
                {
                    // Create new inventory record if it doesn't exist
                    var inventoryItem = new Inventory
                    {
                        itemID = newReuseDestination.Value,
                        quantity = (decimal)newQuantity,
                        createdDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        sUser = "User"
                    };
                    Db.Add(inventoryItem);
                }
            }
        }

        

        private void RemoveReturnItem(ReturnItems item, Assignment4Context Db)
        {
            // Remove quantity from inventory before deleting the item
            if (item.fkReuseDestianationItemId.HasValue && item.fkReuseDestianationItemId > 0)
            {
                var inventoryItem = Db.Inventory.FirstOrDefault(i => i.itemID == item.fkReuseDestianationItemId);
                if (inventoryItem != null)
                {
                    inventoryItem.quantity -= (decimal)(item.quantity ?? 0);
                    inventoryItem.updatedDate = DateTime.Now;
                    inventoryItem.sUser = "User";
                    Db.Update(inventoryItem);
                }
            }

            Db.Remove(item);
        }
        #endregion

        #region DeleteReturn - IMPROVED
        public bool DeleteReturn(int returnId)
        {
            using var Db = new Assignment4Context(_dbConn);
            using var transaction = Db.Database.BeginTransaction();

            try
            {
                // First, remove return items and adjust inventory
                DeleteReturnItemsInternal(returnId, Db);

                // Then remove the return management record
                var returnData = Db.ReturnManagement.FirstOrDefault(r => r.returnId == returnId);
                if (returnData != null)
                {
                    Db.Remove(returnData);
                    Db.SaveChanges();
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in DeleteReturn: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region DeleteReturnItems - IMPROVED
        public bool DeleteReturnItems(int returnManagementId)
        {
            using var Db = new Assignment4Context(_dbConn);
            using var transaction = Db.Database.BeginTransaction();

            try
            {
                DeleteReturnItemsInternal(returnManagementId, Db);
                Db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in DeleteReturnItems: {ex.Message}");
                return false;
            }
        }

        private void DeleteReturnItemsInternal(int returnManagementId, Assignment4Context Db)
        {
            var items = Db.ReturnItems.Where(x => x.fkReturnId == returnManagementId).ToList();

            foreach (var item in items)
            {
                // Remove quantity from inventory before deleting
                if (item.fkReuseDestianationItemId.HasValue && item.fkReuseDestianationItemId > 0)
                {
                    var inventoryItem = Db.Inventory.FirstOrDefault(i => i.itemID == item.fkReuseDestianationItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity -= (decimal)(item.quantity ?? 0);
                        inventoryItem.updatedDate = DateTime.Now;
                        inventoryItem.sUser = "User";
                        Db.Update(inventoryItem);
                    }
                }
            }

            Db.RemoveRange(items);
        }
        #endregion

        #region GetReturnById
        public ReturnManagement GetReturnById(int returnId)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.ReturnManagement.FirstOrDefault(r => r.returnId == returnId);
        }
        #endregion

        #region GetReturnItems
        public List<ReturnItems> GetReturnItems()
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.ReturnItems.ToList();
        }
        #endregion

        #region GetInventoryItems
        public List<InventoryItems> GetInventoryItems()
        {
            using var Db = new Assignment4Context(_dbConn);
            var inventoryItems = Db.InventoryItems.Where(x => x.itemType == "finishedgoods").ToList();
            return inventoryItems;
        }
        #endregion

        #region GetReturns
        public List<ReturnManagementViewModal> GetReturns()
        {
            using (var context = new Assignment4Context(_dbConn))
            {
                var result = (from rm in context.ReturnManagement
                              join c in context.Customers on rm.fkcotomerID equals c.customerId into custGroup
                              from c in custGroup.DefaultIfEmpty() // LEFT JOIN to handle null customers
                              select new ReturnManagementViewModal
                              {
                                  returnId = rm.returnId,
                                  customerId = rm.fkcotomerID ?? 0, // Default to 0 if null
                                  returnDescription = rm.returnDescription,
                                  TotalAmount = rm.TotalReturnPrice ?? 0, // Default to 0 if null
                                  customerName = c != null ? c.customerName : "Unknown", // Handle null customer
                                  returnDate = rm.returnDate,
                                  returnPrice = rm.TotalReturnPrice ?? 0,
                              }).ToList();

                return result;
            }
        }
        #endregion

        #region GetReturnItemsWithItemNameWithID
        public List<ReturnItemViewModel> GetReturnItemsWithItemNameWithID(int returnId)
        {
            using (var context = new Assignment4Context(_dbConn))
            {
                // Step 1: Check if return data exists
                var returnData = context.ReturnItems
                                        .Where(r => r.fkReturnId == returnId)
                                        .FirstOrDefault();

                if (returnData == null)
                {
                    return new List<ReturnItemViewModel>(); // Return empty list if no return data
                }

                // Step 2: Get associated return items with item names
                var returnItems = (from ri in context.ReturnItems
                                   join ii in context.InventoryItems on ri.fkInventoryItemId equals ii.itemId
                                   where ri.fkReturnId == returnId
                                   select new ReturnItemViewModel
                                   {
                                       returnItemTblId = ri.returnItemTblId,
                                       fkInventoryItemId = ri.fkInventoryItemId,
                                       quantity = ri.quantity,
                                       returnReason = ri.returnReason,
                                       returnDescription = ri.returnDescription,
                                       returnPrice = ri.returnPrice,
                                       unit = ri.unit,
                                       discount = ri.discount,
                                       ReuseDestination = ri.ReuseDestination,
                                       fkReuseDestianationItemId = ri.fkReuseDestianationItemId,
                                       itemName = ii.itemName,
                                       PricePerUnit = (decimal)ii.pricePerUnit,
                                   }).ToList();

                return returnItems;
            }
        }
        #endregion
    }
}