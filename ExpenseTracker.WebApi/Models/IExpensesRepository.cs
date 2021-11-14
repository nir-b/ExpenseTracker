using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.WebApi.Models
{
    public interface IExpensesRepository
    {
        /// <summary>
        /// Get all Expenses
        /// </summary>
        IQueryable<Expense> Expenses { get; }

        /// <summary>
        /// Gets an Expense object by Id
        /// </summary>
        /// <param name="id">Expense Id</param>
        /// <returns>Expense object with given Id</returns>
        Task<Expense> GetExpenseById(int id);

        /// <summary>
        /// Adds an Expense object to the repository
        /// </summary>
        /// <param name="expense">The Expense object to add</param>
        /// <returns>The added Expense object</returns>
        Task<Expense> AddExpense(Expense expense);
        
        /// <summary>
        /// Updates an existing Expense object
        /// </summary>
        /// <param name="expense">The Expense object to update</param>
        /// <returns>If an existing Expense object was successfully updated</returns>
        Task<bool> UpdateExpense(Expense expense);

        /// <summary>
        /// Deletes an existing Expense object
        /// </summary>
        /// <param name="id">Id of the Expense object to delete</param>
        /// <returns>If an existing Expense object was successfully deleted</returns>
        Task<bool> DeleteExpense(int id);
    }
}