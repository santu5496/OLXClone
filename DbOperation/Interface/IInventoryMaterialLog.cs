using DbOperation.Models;
using DbOperation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Interface
{
    public interface IInventoryMaterialLog
    {
        bool SavePurchaseData(RawMaterialLogs purchaseData, List<PurchaseDetails> items);
        bool UpdatePurchaseData(RawMaterialLogs purchaseData, List<PurchaseDetails> items);
        bool DeletePurchaseData(int logId);
        //List<dynamic> GetRawMaterialLogs();
        List<PurchaseViewModel> GetPurchaseRecords(DateTime? startDate, DateTime? endDate, List<int> supplierIds, string paymentStatus);
        List<InventoryItems> GetRawMaterialItems();
        List<dynamic> GetPurchasedItemsById(int purchaseId);
    }
}
