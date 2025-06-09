using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class AccountController : Controller
    {
        // 顯示登入頁面
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 處理登入表單提交
        [HttpPost]
        public IActionResult Login(string account, string password, string role)
        {
            if (role == "admin" && account == "1112006" && password == "1234")
            {
                HttpContext.Session.SetString("Role", "admin");
                HttpContext.Session.SetString("Account", account);
                return RedirectToAction("Index", "Expense");  // 導向費用管理頁面
            }
            else if (role == "member" && account == "admin123" && password == "test@1234")
            {
                HttpContext.Session.SetString("Role", "member");
                HttpContext.Session.SetString("Account", account);
                return RedirectToAction("MemberPage", "Account");
            }
            else
            {
                TempData["LoginError"] = "帳號或密碼錯誤";
                return RedirectToAction("Login");
            }
        }

        // 會員頁面
        public IActionResult MemberPage()
        {
            if (HttpContext.Session.GetString("Role") != "member")
                return RedirectToAction("Login");

            ViewBag.Account = HttpContext.Session.GetString("Account");
            return View();
        }

        // 登出
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // 預設轉導到登入頁
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }
    }
}