using ExpenseTracker.Data;
using ExpenseTracker.Data.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 重要：註冊 DbContext 並指定連接字串
builder.Services.AddDbContext<ExpenseTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CmsContext")));

// 註冊服務
builder.Services.AddScoped<IExpensesService, ExpensesService>();

// Session 設定
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // 在 UseRouting 之後
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();