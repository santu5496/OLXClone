using DbOperation.Models;
using DbOperation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Interface
{
    public  interface IOrderManagement

    {
        List<OrdersViewModal> GetAllOrders();
        List<Orders> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        bool DeleteOrder(int orderId);
        bool AddOrder(Orders order, List<BillOrderItems> billOrderItems);
        bool UpdateOrder(Orders order, List<BillOrderItems> billOrderItems);
    }
}
