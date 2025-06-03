using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Layout.Borders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Implementation
{
    
    public class OrderManagementSerivice : IOrderManagement
    {


        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public OrderManagementSerivice(string context)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(context).Options;
        }


        public bool AddOrder(Orders order,List<BillOrderItems> billOrderItems)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                order.createdDate = DateTime.Now;
                order.updatedDate = DateTime.Now;
                order.sUser = "User";
                Db.Add(order);
                Db.SaveChanges();
                var a = order.orderId;
               
                

                foreach (var item in billOrderItems)
                {
                    item.fkBillOrderId = a;
                    item.entryType = "order";
                    item.createdDate = DateTime.Now;
                    item.updatedDate = DateTime.Now;
                    item.sUser = "Admin";
                   

                    Db.Add(item);
                    Db.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateOrder(Orders order, List<BillOrderItems> billOrderItems)
        {
            using var Db = new Assignment4Context(_dbConn);
            using var transaction = Db.Database.BeginTransaction();

            try
            {
                // Find the existing order by orderId
                var existingOrder = Db.Orders.FirstOrDefault(o => o.orderId == order.orderId);
                if (existingOrder == null)
                {
                    return false;
                }

                // Update order details
                existingOrder.fkCustomerId = order.fkCustomerId;
                existingOrder.orderDate = order.orderDate;
                existingOrder.orderPreparationStatus = order.orderPreparationStatus;
                existingOrder.status = order.status;
                existingOrder.totalAmount = order.totalAmount;
                existingOrder.updatedDate = DateTime.Now;
                existingOrder.sUser = "User"; // Set dynamically based on current user if needed

                // Get all existing order items for this order
                var existingItems = Db.BillOrderItems
                    .Where(b => b.fkBillOrderId == order.orderId && b.entryType == "order")
                    .ToList();

                // Track which items are being updated
                var updatedItemIds = new HashSet<int>();

                // Update or add new items
                foreach (var item in billOrderItems)
                {
                    var existingItem = existingItems.FirstOrDefault(ei => ei.fkItemId == item.fkItemId);

                    if (existingItem != null)
                    {
                        // Update existing item
                        existingItem.quantity = item.quantity;
                        existingItem.deliveredQty = item.deliveredQty;
                        existingItem.deliveredStatus = item.deliveredStatus;
                        existingItem.unit = item.unit;
                        existingItem.price = item.price;
                        existingItem.updatedDate = DateTime.Now;
                        existingItem.sUser = "Admin";

                        updatedItemIds.Add(existingItem.fkItemId);
                    }
                    else
                    {
                        // Add new item
                        item.fkBillOrderId = order.orderId;
                        item.entryType = "order";
                        item.createdDate = DateTime.Now;
                        item.updatedDate = DateTime.Now;
                        item.sUser = "Admin";
                        Db.BillOrderItems.Add(item);

                        updatedItemIds.Add(item.fkItemId);
                    }
                }

                // Remove items that are no longer in the updated list
                var itemsToRemove = existingItems.Where(ei => !updatedItemIds.Contains(ei.fkItemId)).ToList();
                if (itemsToRemove.Any())
                {
                    Db.BillOrderItems.RemoveRange(itemsToRemove);
                }

                // Save all changes at once
                Db.SaveChanges();
                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // Log the exception details for debugging
                // Logger.LogError(ex, "Error updating order {OrderId}", order.orderId);
                return false;
            }
        }


        public bool DeleteOrder(int orderId)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                // Find the existing order by orderId
                var order = Db.Orders.FirstOrDefault(o => o.orderId == orderId);
                if (order == null)
                {
                    // If the order doesn't exist, return false
                    return false;
                }

                // Find the order items associated with this order
                var orderItems = Db.BillOrderItems.Where(b => b.fkBillOrderId == orderId).ToList();

                // Remove the order items
                Db.BillOrderItems.RemoveRange(orderItems);

                // Remove the order itself
                Db.Orders.Remove(order);

                // Save changes to the database
                Db.SaveChanges();

                return true;
            }
            catch
            {
                return false; // If an error occurs, return false
            }
        }


        public List<Orders> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.Orders.Where(o => o.orderDate >= startDate && o.orderDate <= endDate).ToList();
        }

        public Orders GetOrderById(int orderId)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.Orders.FirstOrDefault(o => o.orderId == orderId);
        }

      


        public List<OrdersViewModal> GetAllOrders()
        {
            using var Db = new Assignment4Context(_dbConn);

            var query = from order in Db.Orders
                        join customer in Db.Customers
                        on order.fkCustomerId equals customer.customerId into customerGroup
                        from cust in customerGroup.DefaultIfEmpty()
                        select new OrdersViewModal
                        {
                            orderId = order.orderId,
                            fkCustomerId = order.fkCustomerId,
                            orderDate = order.orderDate,
                            totalAmount = order.totalAmount,
                            status = order.status,
                            orderPreparationStatus = order.orderPreparationStatus,
                            createdDate = order.createdDate,
                            updatedDate = order.updatedDate,
                            sUser = order.sUser,
                            Billing = order.Billing,  // Including Billing data
                            fkCustomer = order.fkCustomer,  // Including Customer data
                            customerName = cust != null ? cust.customerName : string.Empty,  // Customer name handling

                            // Include OrderItems filtered by entryType == "order"
                            OrderItems = Db.BillOrderItems
                                .Where(item => item.fkBillOrderId == order.orderId && item.entryType == "order")
                                .Select(item => (dynamic)new
                                {
                                    item.fkBillOrderId,
                                    item.itemId,
                                    item.fkItemId,
                                    item.quantity,
                                    item.unit,
                                    item.price,
                                    item.entryType,
                                    item.deliveredStatus,
                                    item.deliveredQty
                                }).ToList() // Map selected fields for order items
                        };

            return query.ToList();
        }




    }

}
