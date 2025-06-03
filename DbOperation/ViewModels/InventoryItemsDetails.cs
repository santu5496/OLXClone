using DbOperation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class InventoryItemsDetails
    {
        public int inventoryID { get; set; }
        public decimal quantity { get; set; }
        public int itemId { get; set; }

        public string itemName { get; set; }

        public string itemDescription { get; set; }

        public string itemType { get; set; }

        public string unit { get; set; }

        public decimal? pricePerUnit { get; set; }

        public int? fkCategoryId { get; set; }

        public decimal? priceQuantity { get; set; }

        public decimal maxQuantity { get; set; }    

    }

}
