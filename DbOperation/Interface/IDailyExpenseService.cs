using DbOperation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Interface
{
    public interface IDailyExpenseService
    {
        bool AddDailyExpense(DailyExpense expense);

        // Read
        List<DailyExpense> GetAllExpenses();
        DailyExpense GetExpenseById(int id);

        // Update
        bool UpdateDailyExpense(DailyExpense updatedExpense);

        // Delete
        bool DeleteDailyExpense(int id);
    }
}
