using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

public class DailyExpenseController : Controller
{
    private readonly string _filePath;
    private readonly IDailyExpenseService _expenseService;

    public DailyExpenseController(IWebHostEnvironment env,IDailyExpenseService service)
    {
        _filePath = Path.Combine(env.WebRootPath, "jsonFiles", "expenseMasterItem.json");
        _expenseService = service;
    }

    private List<ExpenseItem> LoadData()
    {
        if (!System.IO.File.Exists(_filePath))
            return new List<ExpenseItem>();

        var json = System.IO.File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<List<ExpenseItem>>(json) ?? new List<ExpenseItem>();
    }

    private void SaveData(List<ExpenseItem> items)
    {
        var json = JsonConvert.SerializeObject(items, Formatting.Indented);
        System.IO.File.WriteAllText(_filePath, json);
    }

    public IActionResult GetItems()
    {
        return Ok(LoadData());
    }

    public IActionResult GetItem(int id)
    {
        var items = LoadData();
        var item = items.FirstOrDefault(x => x.ItemID == id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    public IActionResult CreateItem(ExpenseItem newItem)
    {
        try
        {
            if (newItem.ItemID == 0)
            {
                var items = LoadData();
                newItem.ItemID = items.Any() ? items.Max(x => x.ItemID) + 1 : 1;
                items.Add(newItem);
                SaveData(items);
                return Json(true);
            }
            else
            {
                var items = LoadData();
                var item = items.FirstOrDefault(x => x.ItemID == newItem.ItemID);
                if (item == null) return NotFound();

                item.ItemName = newItem.ItemName;
                item.ItemDescription = newItem.ItemDescription;
                SaveData(items);
                return Json(true);
            }
        }
        catch (Exception ex)
        {
            return Json(ex.Message);
        }
    }


    public IActionResult DeleteItem(int id)
    {
        var items = LoadData();
        var item = items.FirstOrDefault(x => x.ItemID == id);
        if (item == null) return NotFound();

        items.Remove(item);
        SaveData(items);

        return NoContent();
    }

    public IActionResult DailyExpense()
    {
        return View();
    }

    public IActionResult AddOrEditDailyExpense(DailyExpense expense)
    {
        try
        {
            if (expense.id == 0)
            {
                var added = _expenseService.AddDailyExpense(expense);
                return Json(new { success = added, message = added ? "Expense added successfully." : "Failed to add expense." });
            }
            else
            {
                var updated = _expenseService.UpdateDailyExpense(expense);
                return Json(new { success = updated, message = updated ? "Expense updated successfully." : "Failed to update expense." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public IActionResult GetAllExpenses()
    {
        try
        {
            var expenses = _expenseService.GetAllExpenses();
            return Json(expenses);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public IActionResult GetExpenseById(int id)
    {
        try
        {
            var expense = _expenseService.GetExpenseById(id);
            if (expense == null)
                return Json(new { success = false, message = "Expense not found." });

            return Json(new { success = true, data = expense });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public IActionResult DeleteExpense(int id)
    {
        try
        {
            var deleted = _expenseService.DeleteDailyExpense(id);
            return Json(new { success = deleted, message = deleted ? "Expense deleted successfully." : "Failed to delete expense." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}

public class ExpenseItem
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
}
