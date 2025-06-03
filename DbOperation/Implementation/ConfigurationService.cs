using System.Reflection.Metadata.Ecma335;
using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
namespace DbOperation.Implementation
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public ConfigurationService(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(dbConn).Options;
        }

        #region InventoryItemsCRUD
        public bool AddInventoryItem(InventoryItems Item)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                Item.createdDate = DateTime.Now;
                Item.updatedDate = DateTime.Now;
                Item.sUser = "User";
                Db.Add(Item);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<ItemMasterView> GetInventoryItems()
        {
            using var Db = new Assignment4Context(_dbConn);

            var inventoryItems = (from item in Db.InventoryItems
                                  join category in Db.Category

                                  on item.fkCategoryId equals category.id
                                  into catGroup
                                  from category in catGroup.DefaultIfEmpty()
                                  select new ItemMasterView
                                  {
                                      itemId = item.itemId,
                                      itemName = item.itemName,
                                      itemDescription = item.itemDescription,
                                      createdDate = item.createdDate,
                                      updatedDate = item.updatedDate,
                                      sUser = item.sUser,
                                      itemType = item.itemType,
                                      unit = item.unit,
                                      pricePerUnit = item.pricePerUnit,
                                      fkCategoryId = item.fkCategoryId,
                                      priceQuantity = item.priceQuantity,
                                      categeoryName = category != null ? category.categoryName : null
                                  }).ToList();

            return inventoryItems;
        }


        public bool DeleteInventoryItem(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var result = Db.InventoryItems.Where(a => a.itemId == id).FirstOrDefault();
                Db.Remove(result);
                Db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateInventoryItem(InventoryItems inventory)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existingInventoryItem = Db.InventoryItems.Find(inventory.itemId);
                if (existingInventoryItem == null)
                {
                    return false;
                }
                existingInventoryItem.itemName = inventory.itemName;
                existingInventoryItem.itemDescription = inventory.itemDescription;
                existingInventoryItem.itemType = inventory.itemType;
                existingInventoryItem.fkCategoryId = inventory.fkCategoryId;
                existingInventoryItem.pricePerUnit = inventory.pricePerUnit;
                existingInventoryItem.unit = inventory.unit;
                existingInventoryItem.priceQuantity = inventory.priceQuantity;
                existingInventoryItem.updatedDate = DateTime.Now;
                Db.InventoryItems.Update(existingInventoryItem);
                Db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion 


        public bool AddSupplier(Suppliers suppliers)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                suppliers.createdDate = DateTime.Now;
                suppliers.updatedDate = DateTime.Now;
                suppliers.sUser = "User";
                Db.Add(suppliers);
                Db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Suppliers> GetSupplierDts()
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.Suppliers.ToList();
        }


        public bool DeleteSupplier(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var result = Db.Suppliers.Where(a => a.supplierId == id).FirstOrDefault();
                Db.Remove(result);
                Db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateSupplier(Suppliers supplier)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existingSupplier = Db.Suppliers.Find(supplier.supplierId);
                if (existingSupplier == null)
                {
                    return false;
                }

                existingSupplier.supplierName = supplier.supplierName;
                existingSupplier.phoneNumber = supplier.phoneNumber;
                existingSupplier.address = supplier.address;
                existingSupplier.state = supplier.state;
                existingSupplier.city = supplier.city;

                Db.Suppliers.Update(existingSupplier);
                Db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool AddCategory(Category cat)
        {
            using var Db = new Assignment4Context(_dbConn);

            try
            {
                cat.createdDate = DateTime.Now;
                cat.updatedDate = DateTime.Now;
                cat.sUser = "User";
                Db.Add(cat);
                Db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Category> GetCategory()
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.Category.ToList();

        }

        public bool UpdateCategory(Category category)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existingCategory = Db.Category.Find(category.id);
                if (existingCategory == null)
                {
                    return false;
                }
                existingCategory.categoryName = category.categoryName;
                existingCategory.description = category.description;
                Db.Category.Update(existingCategory);
                Db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteCategory(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var result = Db.Category.Where(a => a.id == id).FirstOrDefault();
                Db.Remove(result);
                Db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Customer

        public bool AddCustomer(Customers customer)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                customer.createdDate = DateTime.Now;
                customer.updatedDate = DateTime.Now;
                customer.sUser = "User";
                Db.Add(customer);
                Db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Customers> GetCustomers()
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.Customers.ToList();

        }

        public bool UpdateCustomer(Customers customers)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existingCustomer = Db.Customers.Find(customers.customerId);
                if (existingCustomer == null)
                {
                    return false;
                }
                existingCustomer.customerName = customers.customerName;
                existingCustomer.address = customers.address;
                existingCustomer.phoneNo = customers.phoneNo;
                Db.Customers.Update(existingCustomer);
                Db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteCustomer(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var result = Db.Customers.Where(a => a.customerId == id).FirstOrDefault();
                Db.Remove(result);
                Db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public dynamic GetCustomerDetailsWithUnpaidBills(int customerId)
        {
            using var db = new Assignment4Context(_dbConn);

            var customerDetails = db.Customers
                .Where(c => c.customerId == customerId)
                .Select(c => new
                {
                    CustomerName = c.customerName,
                    ID = c.customerId,
                    PhoneNumber = c.phoneNo,
                    Address = c.address,
                    Bills = db.Billing
                        .Where(b => b.fkCustomerId == c.customerId && b.paymentStatus != "Paid")
                        .Select(b => new
                        {
                            BillDate = b.billDate,
                            FinalAmount = b.finalAmount - (b.advanceAmount ?? 0)
                        })
                        .ToList()
                })
                .FirstOrDefault();

            return customerDetails;
        }

    }
}
