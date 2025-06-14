using ExpenseTracker.Data.Service;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpensesService _expensesService;
        private readonly IConfiguration _configuration;

        public ExpenseController(IExpensesService expensesService, IConfiguration configuration)
        {
            _expensesService = expensesService;
            _configuration = configuration;
        }

        // Session 檢查方法
        private bool IsUserLoggedIn()
        {
            var role = HttpContext.Session.GetString("Role");
            var account = HttpContext.Session.GetString("Account");

            // 加入除錯訊息
            ViewBag.DebugSession = $"Role: {role}, Account: {account}";

            return !string.IsNullOrEmpty(role);
        }
        // 重導到登入頁的方法
        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Index(int? page = 1)
        {
            // 檢查 Session 登入狀態
            if (!IsUserLoggedIn())
            {
                return RedirectToLogin();
            }

            ViewBag.UserName = HttpContext.Session.GetString("Account") ?? "訪客";
            ViewBag.UserRole = HttpContext.Session.GetString("Role") ?? "未知";

            const int pageSize = 5;

            try
            {
                
                var debugInfo = new List<string>();

                
                debugInfo.Add($"ExpensesService 注入狀態: {(_expensesService != null ? "成功" : "失敗")}");

                
                var canConnect = await _expensesService.TableExpenses1112006.AnyAsync();
                debugInfo.Add($"資料庫連接狀態: {(canConnect ? "成功" : "失敗")}");

                
                var totalCount = await _expensesService.TableExpenses1112006.CountAsync();
                debugInfo.Add($"資料表總筆數: {totalCount}");

                
                var rawData = await _expensesService.TableExpenses1112006.Take(5).ToListAsync();
                debugInfo.Add($"前5筆原始資料數量: {rawData.Count}");

                if (rawData.Any())
                {
                    debugInfo.Add($"第一筆資料: ID={rawData[0].Id}, 描述={rawData[0].Description}, 金額={rawData[0].Amount}");
                }

                var allExpenses = await _expensesService.TableExpenses1112006
                    .OrderByDescending(e => e.Date)
                    .ThenByDescending(e => e.Id)
                    .ToListAsync();

                debugInfo.Add($"排序後資料數量: {allExpenses.Count}");

                var pagedList = allExpenses.ToPagedList(page ?? 1, pageSize);

                ViewBag.DebugMessage = string.Join(" | ", debugInfo);

                return View(pagedList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "載入費用記錄時發生錯誤：" + ex.Message;
                ViewBag.DebugMessage = "錯誤：" + ex.Message;
                if (ex.InnerException != null)
                {
                    ViewBag.DebugMessage += " | 內部錯誤：" + ex.InnerException.Message;
                }
                return View(new List<Expense>().ToPagedList(1, pageSize));
            }
        }

        public async Task<IActionResult> Index2(int? page = 1)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            const int pageSize = 5;

            try
            {
                var allExpenses = await _expensesService.TableExpenses1112006
                    .OrderByDescending(e => e.Date)
                    .ThenByDescending(e => e.Id)
                    .ToListAsync();

                var pagedList = allExpenses.ToPagedList(page ?? 1, pageSize);
                return View(pagedList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "載入費用記錄時發生錯誤：" + ex.Message;
                return View(new List<Expense>().ToPagedList(1, pageSize));
            }
        }

        public IActionResult Create()
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            if (ModelState.IsValid)
            {
                try
                {
                    await _expensesService.Add(expense);
                    TempData["Success"] = "新增成功！";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    var errorMsg = "新增失敗：" + ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMsg += " | 內部錯誤：" + ex.InnerException.Message;
                    }
                    TempData["Error"] = errorMsg;
                    ModelState.AddModelError("", errorMsg);

                }
            }
            return View(expense);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            if (id == null) return NotFound();

            var expense = await _expensesService.GetById(id.Value);
            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            if (id != expense.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _expensesService.Update(expense);
                    TempData["Success"] = "編輯成功！";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "更新失敗：" + ex.Message;
                }
            }
            return View(expense);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            if (id == null) return NotFound();

            var expense = await _expensesService.GetById(id.Value);
            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            try
            {
                var expense = await _expensesService.GetById(id);
                if (expense != null)
                {
                    await _expensesService.Remove(expense);
                    TempData["Success"] = "刪除成功！";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "刪除失敗：" + ex.Message;
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            if (id == null) return NotFound();

            var expense = await _expensesService.GetById(id.Value);
            if (expense == null) return NotFound();

            return View(expense);
        }

        public IActionResult GetChart()
        {
            if (!IsUserLoggedIn())
            {
                return Json(new { error = "未登入" });
            }

            try
            {
                var data = _expensesService.GetChartData();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        

        public async Task<IActionResult> Search(string description, string category, DateTime? startDate, DateTime? endDate, int? page = 1)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            ViewBag.UserName = HttpContext.Session.GetString("Account") ?? "訪客";
            ViewBag.UserRole = HttpContext.Session.GetString("Role") ?? "未知";

            const int pageSize = 5;

            try
            {
                
                var allCategories = await _expensesService.TableExpenses1112006
                    .Where(e => !string.IsNullOrEmpty(e.Category))
                    .Select(e => e.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                ViewBag.CategoryList = new SelectList(allCategories, category); // Correct usage

                var query = _expensesService.TableExpenses1112006.AsQueryable();

                if (!string.IsNullOrEmpty(description))
                {
                    query = query.Where(e => e.Description.Contains(description));
                    ViewBag.SearchDescription = description;
                }

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(e => e.Category == category);
                    ViewBag.SearchCategory = category;
                }

                if (startDate.HasValue)
                {
                    query = query.Where(e => e.Date >= startDate.Value);
                    ViewBag.SearchStartDate = startDate.Value.ToString("yyyy-MM-dd");
                }

                if (endDate.HasValue)
                {
                    query = query.Where(e => e.Date <= endDate.Value);
                    ViewBag.SearchEndDate = endDate.Value.ToString("yyyy-MM-dd");
                }

                var filteredExpenses = await query
                    .OrderByDescending(e => e.Date)
                    .ThenByDescending(e => e.Id)
                    .ToListAsync();

                var pagedList = filteredExpenses.ToPagedList(page ?? 1, pageSize);

                ViewBag.CurrentSearch = new
                {
                    description = description,
                    category = category,
                    startDate = startDate?.ToString("yyyy-MM-dd"),
                    endDate = endDate?.ToString("yyyy-MM-dd")
                };

                ViewBag.DebugMessage = $"找到 {filteredExpenses.Count} 筆符合條件的記錄";

                return View(pagedList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "查詢費用記錄時發生錯誤：" + ex.Message;
                ViewBag.DebugMessage = "錯誤：" + ex.Message;
                return View(new List<Expense>().ToPagedList(1, pageSize));
            }
        }


        // 清除搜尋條件
        public IActionResult ClearSearch()
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();
            return RedirectToAction("Index");
        }

    }
}