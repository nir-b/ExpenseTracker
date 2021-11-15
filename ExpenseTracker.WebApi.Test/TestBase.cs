using System;
using System.Collections.Generic;
using System.Text;
using ExpenseTracker.WebApi.Models;

namespace ExpenseTracker.WebApi.Test
{
    public class TestBase
    {
        protected Expense[] GetExpenses()
        {
            var timeStamp = DateTime.UtcNow;
            return new[]
            {
                new Expense()
                {
                    Id = 1,
                    Category = "Rent and Mortgage",
                    Title = $"{timeStamp:MMMM}'s Rent",
                    Description = "Roof over our heads",
                    Amount = 1000.0,
                    DateAdded = timeStamp,
                    DateUpdated = timeStamp,
                },
                new Expense()
                {
                    Id = 2,
                    Category = "Groceries",
                    Title = $"Groceries for {timeStamp:MMMM}",
                    Description = "Cooking and Cleaning",
                    Amount = 750.0,
                    DateAdded = timeStamp,
                    DateUpdated = timeStamp,
                },
                new Expense()
                {
                    Id = 3,
                    Category = "Entertainment",
                    Title = $"Movies and Eating out",
                    Description = $"Entertainments costs for {timeStamp:MMMM}",
                    Amount = 500.0,
                    DateAdded = timeStamp,
                    DateUpdated = timeStamp,
                }
            };
        }

        protected Expense GetExpense()
        {
            var timeStamp = DateTime.UtcNow;
            return new Expense()
            {
                Category = "Some Category",
                Title = $"Some Title",
                Description = $"Costs for {timeStamp:MMMM}",
                Amount = 500.0,
                DateAdded = timeStamp,
                DateUpdated = timeStamp,
            };
        }

        protected IEqualityComparer<Expense> GetExpensesComparer()
        {
            return ComparerFactory.GetComparer<Expense>((e1, e2) =>
                e1.Id == e2.Id
                && e1.Title == e2.Title
                && Math.Abs(e1.Amount - e2.Amount) < Double.Epsilon
                && e1.Category == e2.Category
                && e1.Description == e2.Description
                && e1.ExpenseDate == e2.ExpenseDate
                && e1.DateAdded == e2.DateAdded
                && e1.DateUpdated == e2.DateUpdated
            );
        }

    }
}
