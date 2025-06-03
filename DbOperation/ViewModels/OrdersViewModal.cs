using DbOperation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class OrdersViewModal
    {
        public string customerName { get; set; }

        public int orderId { get; set; }

        public int fkCustomerId { get; set; }

        public DateTime orderDate { get; set; }

        public decimal totalAmount { get; set; }

        public string status { get; set; }

        public string orderPreparationStatus { get; set; }

        public DateTime? createdDate { get; set; }

        public DateTime? updatedDate { get; set; }

        public string sUser { get; set; }

        public virtual ICollection<Billing> Billing { get; set; } = new List<Billing>();

        public virtual Customers fkCustomer { get; set; }
       

        // Adding a property for OrderItems
        public virtual ICollection<dynamic> OrderItems { get; set; } = new List<dynamic>();
    }
}
