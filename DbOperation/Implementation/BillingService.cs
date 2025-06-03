using System.Reflection.Metadata.Ecma335;
using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using iText.StyledXmlParser.Node;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
namespace DbOperation.Implementation
{
    public class BillingService : IBillingService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public BillingService(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(dbConn).Options;
        }

        public List<InventoryItemsDetails> GetBillingItemsByCategory(int catId, string search)
        {
            var db = new Assignment4Context(_dbConn);

            var query = from inventoryItem in db.InventoryItems
                        join inventory in db.Inventory
                        on inventoryItem.itemId equals inventory.itemID
                        where inventoryItem.itemType == "finishedgoods"
                        select new InventoryItemsDetails
                        {
                            itemId = inventoryItem.itemId,
                            itemName = inventoryItem.itemName,
                            unit = inventoryItem.unit,
                            pricePerUnit = inventoryItem.pricePerUnit,
                            priceQuantity = inventoryItem.priceQuantity,
                            maxQuantity = inventory.quantity,
                            fkCategoryId = inventoryItem.fkCategoryId // you may need to include this for filtering
                        };

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.itemName.ToLower().Contains(search.ToLower()));
            }
            else
            {
                query = query.Where(x => x.fkCategoryId == catId);
            }

            return query.ToList();
        }

        public List<InventoryItems> GetFinishedGoodsItems()
        {
            var db = new Assignment4Context(_dbConn);
            return db.InventoryItems.Where(x => x.itemType == "finishedgoods").ToList();
        }

        public MasterBranch GetOwnerBranchDetails()
        {
            var db = new Assignment4Context(_dbConn);
            return db.MasterBranch.FirstOrDefault();
        }

        //public InventoryItems GetIventoryItemById(int id)
        //{
        //    var db = new Assignment4Context(_dbConn);
        //    return db.InventoryItems.FirstOrDefault(x => x.itemId == id && x.itemType == "finishedgoods");
        //}
        public InventoryItemsDetails GetInventoryItemById(int id)
        {
            var db = new Assignment4Context(_dbConn);

            var result = (from inventoryItem in db.InventoryItems
                          join inventory in db.Inventory
                          on inventoryItem.itemId equals inventory.itemID
                          where inventoryItem.itemId == id && inventoryItem.itemType == "finishedgoods"
                          select new InventoryItemsDetails
                          {
                              itemId = inventoryItem.itemId,
                              itemName = inventoryItem.itemName,
                              unit = inventoryItem.unit,
                              pricePerUnit = inventoryItem.pricePerUnit,
                              priceQuantity = inventoryItem.priceQuantity,
                              maxQuantity = inventory.quantity // Added Inventory Quantity
                          }).FirstOrDefault();

            return result;
        }

        //public bool SaveBillData(Billing billData, List<BillOrderItems> items,int orderId)
        //{
        //    try
        //    {
        //        var db = new Assignment4Context(_dbConn);

        //        // Insert Bill Data
        //        billData.billDate = DateTime.Now;
        //        billData.createdDate = DateTime.Now;
        //        billData.updatedDate = DateTime.Now;
        //        billData.sUser = "Admin";
        //        db.Add(billData);
        //        db.SaveChanges();

        //        // Process Bill Order Items
        //        foreach (var item in items)
        //        {
        //            var BillItems = new BillOrderItems
        //            {
        //                fkBillOrderId = billData.billId,
        //                entryType = "bill",
        //                quantity = item.quantity,
        //                price = item.price,
        //                unit = item.unit,
        //                fkItemId = item.fkItemId,
        //                createdDate = DateTime.Now,
        //                updatedDate = DateTime.Now,
        //                sUser = "Admin"
        //            };
        //            db.Add(BillItems);

        //            // Subtract from Inventory
        //            var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == item.fkItemId);
        //            if (inventoryItem != null)
        //            {
        //                inventoryItem.quantity -= item.quantity;
        //                inventoryItem.updatedDate = DateTime.Now;
        //                //if (inventoryItem.quantity < 0)
        //                //{
        //                //    inventoryItem.quantity = 0;
        //                //}

        //                db.Update(inventoryItem);
        //            }
        //        }
        //        if(orderId != 0)
        //        {
        //            var result =db.BillOrderItems
        //        }
        //        db.SaveChanges();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return false;
        //    }
        //}
        public bool SaveBillData(Billing billData, List<BillOrderItems> items, int orderId)
        {
            try
            {
                var db = new Assignment4Context(_dbConn);

                // Insert Bill Data
                billData.billDate = DateTime.Now;
                billData.createdDate = DateTime.Now;
                billData.updatedDate = DateTime.Now;
                billData.sUser = "Admin";
                db.Add(billData);
                db.SaveChanges();

                // Process Bill Order Items
                foreach (var item in items)
                {
                    var BillItems = new BillOrderItems
                    {
                        fkBillOrderId = billData.billId,
                        entryType = "bill",
                        quantity = item.quantity,
                        price = item.price,
                        unit = item.unit,
                        fkItemId = item.fkItemId,
                        createdDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        sUser = "Admin"
                    };
                    db.Add(BillItems);

                    // Subtract from Inventory
                    var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == item.fkItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity -= item.quantity;
                        inventoryItem.updatedDate = DateTime.Now;
                        db.Update(inventoryItem);
                    }
                }

                // If orderId is present, update quantities from existing order items
                if (orderId != 0)
                {
                    var existingOrderItems = db.BillOrderItems
                        .Where(b => b.fkBillOrderId == orderId && b.entryType == "order")
                        .ToList();

                    foreach (var orderItem in existingOrderItems)
                    {
                        var matchingItem = items.FirstOrDefault(i => i.fkItemId == orderItem.fkItemId);
                        if (matchingItem != null)
                        {
                            orderItem.deliveredQty += matchingItem.quantity;
                            orderItem.deliveredStatus = orderItem.deliveredQty == orderItem.quantity  ? "Delivered" : "Partial";
                            orderItem.updatedDate = DateTime.Now;
                            db.Update(orderItem);
                            db.SaveChanges();
                        }
                    }

                    var isOrderCompleted = existingOrderItems.Any(i => i.deliveredStatus != "Partial");
                    if (isOrderCompleted) { 
                        var orderData = db.Orders.FirstOrDefault(o => o.orderId == orderId);
                        orderData.status = "Completed";
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

        public bool UpdateBillData(Billing billData, List<BillOrderItems> items)
        {
            try
            {
                var db = new Assignment4Context(_dbConn);

                // Update Bill Data
                var existingBill = db.Billing.FirstOrDefault(b => b.billId == billData.billId);
                if (existingBill != null)
                {
                    existingBill.fkCustomerId = billData.fkCustomerId;
                    existingBill.finalAmount = billData.finalAmount;
                    existingBill.advanceAmount = billData.advanceAmount;
                    existingBill.paymentMode = billData.paymentMode;
                    existingBill.paymentStatus = billData.paymentStatus;
                    existingBill.discount = billData.discount;
                    existingBill.taxAmount = billData.taxAmount;
                    existingBill.paymentStatus = billData.paymentStatus;
                    existingBill.billDate = DateTime.Now;
                    existingBill.updatedDate = DateTime.Now;
                    existingBill.sUser = "Admin";
                    db.Update(existingBill);
                }

                var existingItems = db.BillOrderItems.Where(b => b.fkBillOrderId == billData.billId && b.entryType=="bill").ToList();
                foreach (var existingItem in existingItems)
                {
                    var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == existingItem.fkItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity += existingItem.quantity; // Restore previous quantity
                        inventoryItem.updatedDate = DateTime.Now;
                        db.Update(inventoryItem);
                    }
                    db.BillOrderItems.Remove(existingItem);
                }

                // Process New Bill Order Items
                foreach (var item in items)
                {
                    var newItem = new BillOrderItems
                    {
                        fkBillOrderId = billData.billId,
                        entryType = "bill",
                        quantity = item.quantity,
                        price = item.price,
                        unit = item.unit,
                        fkItemId = item.fkItemId,
                        createdDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        sUser = "Admin"
                    };
                    db.Add(newItem);

                    var inventoryItem = db.Inventory.FirstOrDefault(i => i.itemID == item.fkItemId);
                    if (inventoryItem != null)
                    {
                        inventoryItem.quantity -= item.quantity;
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

        public List<BillingViewModel> GetBillRecords(DateTime? startDate, DateTime? endDate, string paymentStatus, List<int> customerIds)
        {
            using var db = new Assignment4Context(_dbConn);

            var query = from bill in db.Billing
                        join customer in db.Customers
                        on bill.fkCustomerId equals customer.customerId into custGroup
                        from cust in custGroup.DefaultIfEmpty()
                        select new BillingViewModel
                        {
                            billId = bill.billId,
                            fkOrderId = bill.fkOrderId,
                            fkCustomerId = bill.fkCustomerId,
                            paymentMode = bill.paymentMode,
                            discount = bill.discount,
                            taxAmount = bill.taxAmount,
                            finalAmount = bill.finalAmount,
                            paymentStatus = bill.paymentStatus,
                            billDate = bill.billDate,
                            createdDate = bill.createdDate,
                            sUser = bill.sUser,
                            customerName = cust != null ? cust.customerName : string.Empty
                        };

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(b => b.billDate >= startDate.Value && b.billDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query = query.Where(b => b.paymentStatus == paymentStatus);
            }

            if (customerIds != null && customerIds.Any())
            {
                query = query.Where(b => customerIds.Contains(b.fkCustomerId ?? 0));
            }

            return query.ToList();
        }

        public List<BillItemsDetails> GetBillItemsByBillId(int billId)
        {
            var db = new Assignment4Context(_dbConn);

            var result = (from billItem in db.BillOrderItems
                          join inventoryItem in db.InventoryItems
                              on billItem.fkItemId equals inventoryItem.itemId
                          join inventory in db.Inventory
                              on inventoryItem.itemId equals inventory.itemID
                          where billItem.fkBillOrderId == billId && billItem.entryType == "bill"
                          select new BillItemsDetails
                          {
                              itemId = billItem.fkItemId,
                              itemName = inventoryItem.itemName,
                              unit = inventoryItem.unit,
                              pricePerUnit = inventoryItem.pricePerUnit,
                              priceQuantity = inventoryItem.priceQuantity,
                              fkBillOrderId = billItem.fkBillOrderId,
                              fkItemId = billItem.fkItemId,
                              quantity = billItem.quantity,
                              price = billItem.price,
                              maxQuantity = inventory.quantity
                          }).ToList();

            return result;
        }


        public List<dynamic> fetchOrderDetailsByCustomer(int custmerId)
        {
            var db = new Assignment4Context(_dbConn);

            var orderDetails = (from order in db.Orders
                                where order.fkCustomerId == custmerId && order.status == "Pending"
                                select new
                                {
                                    orderId = order.orderId,
                                    orderDate = order.orderDate, // Example of assigning DateTime
                                    totalAmount = order.totalAmount > 0 ? order.totalAmount : 0,
                                    status = !string.IsNullOrEmpty(order.status) ? order.status : "Pending",
                                    orderPreparationStatus = order.orderPreparationStatus,
                                    BillOrderItems = (from b in db.BillOrderItems
                                                      join i in db.InventoryItems
                                                      on b.fkItemId equals i.itemId
                                                      where b.fkBillOrderId == order.orderId && b.entryType == "order"
                                                      select new
                                                      {
                                                          itemId = b.itemId,
                                                          fkItemId = b.fkItemId,
                                                          quantity = b.quantity > 0 ? b.quantity : 1,
                                                          unit = b.unit,
                                                          price = b.price > 0 ? b.price : 0,
                                                          entryType = b.entryType,
                                                          deliveredStatus = b.deliveredStatus,
                                                          deliveredQty = b.deliveredQty ?? 0,
                                                          remainingQty = b.quantity - (b.deliveredQty ?? 0),
                                                          pricePerUnit = i.pricePerUnit,
                                                          priceQuantity = i.priceQuantity,
                                                          itemName = i.itemName
                                                      }).ToList()
                                }).ToList<dynamic>();

            return orderDetails;
        }

    }
}
