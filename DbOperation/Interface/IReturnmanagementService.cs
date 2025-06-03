using DbOperation.Models;
using DbOperation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Interface
{
    public interface IReturnmanagementService
    {
        ReturnManagement GetReturnById(int returnId);
       
        bool DeleteReturn(int returnId);
        bool UpdateReturn(ReturnManagement returnData, List<ReturnItems> returnItems);
        bool AddReturn(ReturnManagement returnData, List<ReturnItems> returnItems);
        List<InventoryItems> GetInventoryItems();
        List<ReturnManagementViewModal> GetReturns();
        List<ReturnItemViewModel> GetReturnItemsWithItemNameWithID(int returnId);

    }
}
