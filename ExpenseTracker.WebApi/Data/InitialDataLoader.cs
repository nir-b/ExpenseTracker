using System;
using System.Linq;
using ExpenseTracker.WebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.WebApi.Data
{
    public static class InitialDataLoader
    {
        public static void EnsureDatabaseSetup(IApplicationBuilder app)
        {
            var expensesDbContext = app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ExpensesDbContext>();

            if (expensesDbContext.Database.GetPendingMigrations().Any()) expensesDbContext.Database.Migrate();

            var timeStamp = DateTime.UtcNow;

            if (!expensesDbContext.Expenses.Any())
            {
                expensesDbContext
                    .AddRents(timeStamp)
                    .AddGroceries(timeStamp)
                    .AddTravel(timeStamp)
                    .AddEntertainment(timeStamp);

                expensesDbContext.SaveChanges();
            }
        }

        #region Helper methods for DB population

        private static ExpensesDbContext AddRents(this ExpensesDbContext expensesDbContext, DateTime timeStamp)
        {
            return expensesDbContext
                .AddMonthlyExpensesForAYear(date => new Expense
                {
                    Category = "Rent and Mortgage",
                    Title = $"{date:MMMM}'s Rent",
                    Description = "Roof over our heads",
                    Amount = 1000.0,
                    DateAdded = timeStamp,
                    DateUpdated = timeStamp
                }, timeStamp)
                .AddMonthlyExpensesForAYear(date => new Expense
                    {
                        Category = "Rent and Mortgage",
                        Title = $"{date:MMMM}'s Mortgage",
                        Description = "More roof over our heads",
                        Amount = 2000.0,
                        DateAdded = timeStamp,
                        DateUpdated = timeStamp
                    }, timeStamp
                );
        }

        private static ExpensesDbContext AddGroceries(this ExpensesDbContext expensesDbContext, DateTime timeStamp)
        {
            return expensesDbContext
                .AddMonthlyExpensesForAYear(date => new Expense
                {
                    Category = "Groceries",
                    Title = $"Groceries for {date:MMMM}",
                    Description = "Cooking and Cleaning",
                    Amount = 1000.0,
                    DateAdded = timeStamp,
                    DateUpdated = timeStamp
                }, timeStamp, true);
        }

        private static ExpensesDbContext AddTravel(this ExpensesDbContext expensesDbContext, DateTime timeStamp)
        {
            return expensesDbContext
                .AddMonthlyExpensesForAYear(date => new Expense
                {
                    Category = "Travel",
                    Title = "Travel",
                    Description = $"Travel costs for {date:MMMM}",
                    Amount = 500.0,
                    DateAdded = timeStamp,
                    DateUpdated = timeStamp
                }, timeStamp, true);
        }


        private static ExpensesDbContext AddEntertainment(this ExpensesDbContext expensesDbContext, DateTime timeStamp)
        {
            return expensesDbContext
                .AddMonthlyExpensesForAYear(date => new Expense
                {
                    Category = "Entertainment",
                    Title = "Movies and Eating out",
                    Description = $"Entertainments costs for {date:MMMM}",
                    Amount = 500.0,
                    DateAdded = timeStamp,
                    DateUpdated = timeStamp
                }, timeStamp, true);
        }

        private static ExpensesDbContext AddMonthlyExpensesForAYear(this ExpensesDbContext expensesDbContext,
            Func<DateTime, Expense> expenseCreator, DateTime timeStamp, bool isRandomAmount = false)
        {
            var random = new Random();
            for (var counter = 0; counter < 12; counter++)
            {
                var expenseDate = new DateTime(timeStamp.Year, timeStamp.Month, 1).AddMonths(-counter);
                var expense = expenseCreator(expenseDate);
                expense.ExpenseDate = expenseDate;
                if (isRandomAmount) expense.Amount *= random.NextDouble();
                expensesDbContext.Expenses.Add(expense);
            }

            return expensesDbContext;
        }

        #endregion
    }
}