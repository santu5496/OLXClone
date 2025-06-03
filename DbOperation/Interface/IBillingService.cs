using DbOperation.Models;
using DbOperation.ViewModels;

namespace DbOperation.Interface
{
    public interface IBillingService
    {
        List<InventoryItemsDetails> GetBillingItemsByCategory(int catId, string search);
        List<InventoryItems> GetFinishedGoodsItems();
        MasterBranch GetOwnerBranchDetails();

        InventoryItemsDetails GetInventoryItemById(int id);
        bool SaveBillData(Billing billData, List<BillOrderItems> items, int orderId);
        bool UpdateBillData(Billing billData, List<BillOrderItems> items);

        List<BillingViewModel> GetBillRecords(DateTime? startDate, DateTime? endDate, string paymentStatus, List<int> customerIds);
        List<BillItemsDetails> GetBillItemsByBillId(int billId);
        List<dynamic> fetchOrderDetailsByCustomer(int custmerId);
    }

}
