using DbOperation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Interface
{
    public interface IFinishedGoodsService
    {
        bool AddFinishedGoods(BakedCookedLogs finishedGoods, List<ItemsUsedForGoods> items);
        bool UpdateFinishedGoods(BakedCookedLogs finishedGoods, List<ItemsUsedForGoods> items);
        bool DeleteFinishedGoods(int id);
        List<FinishedGoodsItems> GetFinishedGoods(DateTime startDate, DateTime endDate);
        dynamic GetInventoryItemsQuantityById(int id);
        List<ItemsUsedForGoods> GetItemsUsedforItem(int id, int baseQuantity);

        List<InventoryItems> GetFinishedGoodsItems();
        List<ItemsUsedForGoods> GetIngredientsUsedByLogId(int logId);
    }
}
