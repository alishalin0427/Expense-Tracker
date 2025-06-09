using ExpenseTracker.Data;
using ExpenseTracker.Data.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ���n�G���U DbContext �ë��w�s���r��
builder.Services.AddDbContext<ExpenseTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CmsContext")));

// ���U�A��
builder.Services.AddScoped<IExpensesService, ExpensesService>();

// Session �]�w
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
app.UseSession(); // �b UseRouting ����
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();