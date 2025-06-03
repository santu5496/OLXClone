using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class ReturnItemViewModel
    {
        public int returnItemTblId { get; set; }

        public int? fkInventoryItemId { get; set; }

        public int? quantity { get; set; }

        public string returnReason { get; set; }

        public string returnDescription { get; set; }

        public decimal? returnPrice { get; set; }

        public int fkReturnId { get; set; }

        public string unit { get; set; }

        public decimal discount { get; set; }

        public string ReuseDestination { get; set; }

        public int? fkReuseDestianationItemId { get; set; }
      
        public int itemId { get; set; }
        public string itemName { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PriceQuantity { get; set; }




    }

}
