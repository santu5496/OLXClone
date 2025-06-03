using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using DbOperation.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbOperation.Implementation
{
    public class FinishedGoodsService : IFinishedGoodsService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public FinishedGoodsService(string context)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(context).Options;
        }


        public bool AddFinishedGoods(BakedCookedLogs finishedGoods,List<ItemsUsedForGoods> items)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                finishedGoods.createdDate = DateTime.Now;
                finishedGoods.updatedDate = DateTime.Now;
                finishedGoods.sUser = "User";
                Db.Add(finishedGoods);
                Db.SaveChanges();
                var IsInventoryItemExist = Db.Inventory.Where(a => a.itemID == finishedGoods.fkItemId).FirstOrDefault();
                if (IsInventoryItemExist != null)
                {
                    IsInventoryItemExist.quantity += finishedGoods.actualQuantity;
                }
                else
                {
                    Inventory Inventory = new Inventory();
                    Inventory.itemID = finishedGoods.fkItemId;
                    Inventory.quantity = finishedGoods.actualQuantity;
                    Inventory.createdDate = DateTime.Now;
                    Inventory.updatedDate = DateTime.Now;
                    Inventory.sUser = "User";
                    Db.Add(Inventory);
                    Db.SaveChanges();
                }
                foreach (var item in items)
                {
                    var itemsUsed = new ItemsUsedForGoods();
                    itemsUsed.fkMasterItemId = item.fkMasterItemId;
                    itemsUsed.fkBakeRecId = finishedGoods.logId;
                    itemsUsed.quantity = item.quantity;
                    itemsUsed.unit = item.unit;
                    itemsUsed.goodsType = "finishedgoods";
                    Db.Add(itemsUsed);

                    // Update inventory based on the item used
                    var inventoryItem = Db.Inventory.FirstOrDefault(a => a.itemID == item.fkMasterItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity -= item.quantity; // Reduce quantity from inventory
                    }
                    else
                    {
                        Inventory newInventoryItem = new Inventory();
                        newInventoryItem.itemID = item.fkMasterItemId;
                        newInventoryItem.quantity = -item.quantity; // Add as a negative quantity
                        newInventoryItem.createdDate = DateTime.Now;
                        newInventoryItem.updatedDate = DateTime.Now;
                        newInventoryItem.sUser = "User";
                        Db.Add(newInventoryItem);
                    }
                    Db.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateFinishedGoods(BakedCookedLogs finishedGoods, List<ItemsUsedForGoods> items)
        {
            using var Db = new Assignment4Context(_dbConn);

            var result = Db.BakedCookedLogs.Where(a => a.logId == finishedGoods.logId).FirstOrDefault();
            if (result != null)
            {
                var lastUpdatedQuantity = result.actualQuantity;
                result.fkItem = finishedGoods.fkItem;
                result.targetQuantity = finishedGoods.targetQuantity;
                result.actualQuantity = finishedGoods.actualQuantity;
                result.reasonForDiff = finishedGoods.reasonForDiff;
                result.updatedDate = DateTime.Now;
                result.sUser = "User";
                Db.SaveChanges();

                var IsInventoryItemExist = Db.Inventory.Where(a => a.itemID == finishedGoods.fkItemId).FirstOrDefault();
                if (IsInventoryItemExist != null)
                {
                    IsInventoryItemExist.quantity -= lastUpdatedQuantity;
                    IsInventoryItemExist.quantity += finishedGoods.actualQuantity;
                }
                else
                {
                    Inventory Inventory = new Inventory();
                    Inventory.itemID = finishedGoods.fkItemId;
                    Inventory.quantity = finishedGoods.actualQuantity;
                    Inventory.createdDate = DateTime.Now;
                    Inventory.updatedDate = DateTime.Now;
                    Inventory.sUser = "User";
                    Db.Add(Inventory);
                    Db.SaveChanges();
                }

                var existingItems = Db.ItemsUsedForGoods.Where(a => a.fkBakeRecId == finishedGoods.logId && a.goodsType == "finishedgoods").ToList();

                foreach (var existingItem in existingItems)
                {
                    var inventoryItem = Db.Inventory.FirstOrDefault(a => a.itemID == existingItem.fkMasterItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity += existingItem.quantity; // Revert previously deducted quantity
                    }
                }

                if (existingItems.Any())
                {
                    Db.RemoveRange(existingItems);
                    Db.SaveChanges();
                }

                foreach (var item in items)
                {
                    var itemsUsed = new ItemsUsedForGoods
                    {
                        fkMasterItemId = item.fkMasterItemId,
                        fkBakeRecId = finishedGoods.logId,
                        quantity = item.quantity,
                        unit = item.unit,
                        goodsType = "finishedgoods"
                    };
                    Db.Add(itemsUsed);
                    Db.SaveChanges();
                    // Update inventory by reducing the used quantity
                    var inventoryItemData = Db.Inventory.FirstOrDefault(a => a.itemID == item.fkMasterItemId);
                    if (inventoryItemData != null)
                    {
                        inventoryItemData.quantity -= item.quantity; // Deduct new quantity
                    }
                  
                    else
                    {
                        Inventory newInventory = new Inventory
                        {
                            itemID = item.fkMasterItemId,
                            quantity = -item.quantity,
                            createdDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            sUser = "User"
                        };
                        Db.Add(newInventory);
                    }
                    Db.SaveChanges();
                }

                Db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool DeleteFinishedGoods(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var result = Db.BakedCookedLogs.Where(a => a.logId == id).FirstOrDefault();
                Db.Remove(result);
                
                var IsInventoryItemExist = Db.Inventory.Where(a => a.itemID == id).FirstOrDefault();
                if (IsInventoryItemExist != null)
                {
                    IsInventoryItemExist.quantity -= result.actualQuantity;
                }
                var existingItems = Db.ItemsUsedForGoods.Where(a => a.fkBakeRecId == id && a.goodsType == "finishedgoods").ToList();

                foreach (var existingItem in existingItems)
                {
                    var inventoryItem = Db.Inventory.FirstOrDefault(a => a.itemID == existingItem.fkMasterItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity += existingItem.quantity; // Revert previously deducted quantity
                    }
                }

                if (existingItems.Any())
                {
                    Db.RemoveRange(existingItems);
                }
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<FinishedGoodsItems> GetFinishedGoods(DateTime startDate,DateTime endDate)
        {
            var db = new Assignment4Context(_dbConn);
            var result = db.BakedCookedLogs.Join(db.InventoryItems, a => a.fkItemId, b => b.itemId, (a, b) => new FinishedGoodsItems
            {
                logId = a.logId,
                itemId = a.fkItemId,
                targetQuantity = a.targetQuantity,
                actualQuantity = a.actualQuantity,
                reasonForDiff = a.reasonForDiff,
                createdDate = a.createdDate,
                updatedDate = a.updatedDate,
                sUser = a.sUser,
                itemName = b.itemName
            }).Where(a=>a.createdDate>= startDate && a.createdDate<= endDate).ToList();

            return result;
        }

            public dynamic GetInventoryItemsQuantityById(int id)
        {
            var db = new Assignment4Context(_dbConn);

            var result = (from inv in db.Inventory
                          join item in db.InventoryItems on inv.itemID equals item.itemId
                          where inv.itemID == id
                          select new
                          {
                              quantity = inv.quantity,
                              unit = item.unit
                          }).FirstOrDefault();

            return result != null ? result : 0;
        }

        public List<ItemsUsedForGoods> GetItemsUsedforItem(int id, int baseQuantity)
        {
            var db = new Assignment4Context(_dbConn);
            var recipeData = db.Recipe.FirstOrDefault(x => x.fkItemName == id);

            if (recipeData == null)
            {
                return new List<ItemsUsedForGoods>();
            }

            var itemsUsed = db.ItemsUsedForGoods
                              .Where(x => x.fkBakeRecId == recipeData.recipeId && x.goodsType == "recipe")
                              .ToList();

            // Scale item quantities based on baseQuantity
            foreach (var item in itemsUsed)
            {
                item.quantity = Convert.ToDecimal( (item.quantity / recipeData.baseQuantity) * baseQuantity);
            }

            return itemsUsed;
        }


        public List<InventoryItems> GetFinishedGoodsItems()
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.InventoryItems.Where(a=> a.itemType== "finishedgoods").ToList();

        }

        public List<ItemsUsedForGoods> GetIngredientsUsedByLogId(int logId)
        {
            var db = new Assignment4Context(_dbConn);
            return db.ItemsUsedForGoods.Where(x => x.fkBakeRecId == logId).ToList();
        }
    }
}
