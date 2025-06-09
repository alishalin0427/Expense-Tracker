using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data.Service
{
    public interface IExpensesService
    {
        Task<IEnumerable<Expense>> GetAll();
        Task Add(Expense expense);
        Task Update(Expense expense);
        Task<Expense?> GetById(int id);
        Task Remove(Expense expense);
        Task<Expense?> FindAsync(int id);
        IQueryable<Expense> TableExpenses1112006 { get; }
       
        IQueryable GetChartData();
        
    }
}