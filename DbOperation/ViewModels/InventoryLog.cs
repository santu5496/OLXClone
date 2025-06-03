using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class RawMaterialLogViewModel
    {
        public int LogId { get; set; }
       
        public string SupplierName { get; set; }
        public int supplierId { get; set; }
        public int itemID { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string User { get; set; }
    }

}
