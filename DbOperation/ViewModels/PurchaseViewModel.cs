using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public partial class PurchaseViewModel
    {
        public int purchaseId { get; set; }
        public int? fkOrderId { get; set; }
        public int? fkSupplierId { get; set; }
        public DateTime? purchaseDate { get; set; }
        public decimal? totalAmount { get; set; }
        public string paymentStatus { get; set; }
        public string paymentMode { get; set; }
        public DateTime? createdDate { get; set; }
        public string sUser { get; set; }
        public string supplierName { get; set; }
    }

}
