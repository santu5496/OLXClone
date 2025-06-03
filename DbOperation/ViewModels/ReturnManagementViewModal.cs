using DbOperation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class ReturnManagementViewModal
    {
        public int returnId { get; set; }
        public string customerName { get; set; }
        public int customerId { get; set; }

        public int itemId { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal quantity { get; set; }

        public string unit { get; set; }

        public string returnReason { get; set; }

        public string returnDescription { get; set; }

        public string returnCategory { get; set; }

        public string reuseDestination { get; set; }

        public decimal? returnDiscount { get; set; }

        public decimal returnPrice { get; set; }

        public DateTime? returnDate { get; set; }

        public DateTime? createdDate { get; set; }

        public DateTime? updatedDate { get; set; }

        public string sUser { get; set; }
        public string itemName { get; set; }

        // Add the list of return items for the same returnId
        public List<ReturnItemViewModel> Items { get; set; } // New property

        // If needed, keep the reuse destination name separately
        public string reUseDestinationtName { get; set; }
    }
}
