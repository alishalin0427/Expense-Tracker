using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data.Service
{
    public class ExpensesService : IExpensesService
    {
        private readonly ExpenseTrackerContext _context;

        public ExpensesService(ExpenseTrackerContext context)
        {
            _context = context;
        }

        public async Task Add(Expense expense)
        {
            _context.TableExpenses1112006.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Expense>> GetAll()
        {
            return await _context.TableExpenses1112006.ToListAsync();
        }

        public IQueryable GetChartData()
        {
            var data = _context.TableExpenses1112006
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                });
            return data;
        }

        public async Task Update(Expense expense)
        {
            _context.TableExpenses1112006.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task<Expense?> FindAsync(int id)
        {
            return await _context.TableExpenses1112006.FindAsync(id);
        }

        public async Task<Expense?> GetById(int id)
        {
            return await _context.TableExpenses1112006.FindAsync(id);
        }

        public async Task Remove(Expense expense)
        {
            _context.TableExpenses1112006.Remove(expense);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Expense> TableExpenses1112006 => _context.TableExpenses1112006.AsQueryable();
        
    }
}