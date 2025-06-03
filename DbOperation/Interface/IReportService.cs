using DbOperation.Models;

namespace DbOperation.Interface
{
    public interface IReportService
    {
        List<dynamic> GetTotalPurchased(DateTime reportDate);
        List<dynamic> GetTotalProduced(DateTime reportDate);
        List<dynamic> GetTotalSales(DateTime reportDate);

        List<dynamic> GetPurchaseReports(DateTime startDate, DateTime endDate);
        Task<List<dynamic>> GetBillReportAsync(DateTime startDate, DateTime endDate);

        List<dynamic> GetInventoryReport();
        List<dynamic> GetCookedLogsChart(DateTime startDate, DateTime endDate);
        List<dynamic> GetCookedLogDetailsByDate(DateTime date);
        bool UpdateInventoryQuantities(List<(int id, decimal quantity)> updateList);
    }

}
