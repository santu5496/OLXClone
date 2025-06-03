using DbOperation.Models;

namespace DbOperation.Interface
{
    public interface IConfigurationService
    {
        bool AddInventoryItem(InventoryItems Item);

        List<ItemMasterView> GetInventoryItems();

        bool DeleteInventoryItem(int id);

        bool UpdateInventoryItem(InventoryItems Item);

        bool AddSupplier(Suppliers suppliers);

        List<Suppliers> GetSupplierDts();

        bool DeleteSupplier(int id);

        bool UpdateSupplier(Suppliers suppliers);

        bool AddCategory(Category cat);

        List<Category> GetCategory();

        bool UpdateCategory(Category category);

        bool DeleteCategory(int id);

        bool AddCustomer(Customers customer);

        List<Customers> GetCustomers();

        bool UpdateCustomer(Customers customer);

        bool DeleteCustomer(int id);

        dynamic GetCustomerDetailsWithUnpaidBills(int customerId);
    }

}
