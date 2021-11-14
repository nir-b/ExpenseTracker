using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.WebApi.Models
{
    public class ExpensesRepository : IExpensesRepository
    {
        private readonly ExpensesDbContext _dbContext;
        private readonly ILogger<ExpensesRepository> _logger;

        public ExpensesRepository(ILogger<ExpensesRepository> logger, ExpensesDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IQueryable<Expense> Expenses => _dbContext.Expenses;

        public async Task<Expense> GetExpenseById(int id)
        {
            return await _dbContext.Expenses.FindAsync(id);
        }

        public async Task<Expense> AddExpense(Expense expense)
        {
            if (expense != null)
            {
                _dbContext.Expenses.Add(expense);
                await _dbContext.SaveChangesAsync();
            }
            return expense;
        }

        public async Task<bool> UpdateExpense(Expense expense)
        {
            if (expense != null)
            {
                _dbContext.Entry(expense).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException exp)
                {
                    _logger.LogError($"Unable to update expense object with expense id: {expense?.Id}", exp);
                }
            }

            return false;
        }

        public async Task<bool> DeleteExpense(int id)
        {
            var expense = await _dbContext.Expenses.FindAsync(id);
            if (expense != null)
            {
                expense.DateDeleted = DateTime.UtcNow;

                _dbContext.Entry(expense).State = EntityState.Modified;

                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}