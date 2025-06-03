using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Implementation
{
    public class DailyExpenseService : IDailyExpenseService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;
        public DailyExpenseService(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(dbConn).Options;

        }

        public bool AddDailyExpense(DailyExpense expense)
        {
            using var db = new Assignment4Context(_dbConn);
            try
            {
                expense.createdDate = DateTime.Now;
                expense.updatedDate = DateTime.Now;
                expense.sUser = "User"; // Set from session or login
                db.DailyExpense.Add(expense);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // READ (All)
        public List<DailyExpense> GetAllExpenses()
        {
            using var db = new Assignment4Context(_dbConn);
            return db.DailyExpense.OrderByDescending(x => x.expenseDate).ToList();
        }

        public DailyExpense GetExpenseById(int id)
        {
            using var db = new Assignment4Context(_dbConn);
            return db.DailyExpense.FirstOrDefault(x => x.id == id);
        }

        public bool UpdateDailyExpense(DailyExpense updatedExpense)
        {
            using var db = new Assignment4Context(_dbConn);
            try
            {
                var existing = db.DailyExpense.FirstOrDefault(x => x.id == updatedExpense.id);
                if (existing == null) return false;

                existing.expenseDate = updatedExpense.expenseDate;
                existing.amountGiven = updatedExpense.amountGiven;
                existing.expenseDetails = updatedExpense.expenseDetails;
                existing.totalExpenses = updatedExpense.totalExpenses;
                existing.amountSpent = updatedExpense.amountSpent;
                existing.updatedDate = DateTime.Now;

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteDailyExpense(int id)
        {
            using var db = new Assignment4Context(_dbConn);
            try
            {
                var item = db.DailyExpense.FirstOrDefault(x => x.id == id);
                if (item == null) return false;

                db.DailyExpense.Remove(item);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
