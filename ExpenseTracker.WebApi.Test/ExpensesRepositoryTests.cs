using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ExpenseTracker.WebApi.Controllers;
using ExpenseTracker.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ExpenseTracker.WebApi.Test
{
    public class ExpensesRepositoryTests : TestBase
    {
        private readonly DbContextOptions<ExpensesDbContext> _options;

        public ExpensesRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ExpensesDbContext>()
                .UseInMemoryDatabase(databaseName: "Expenses")
                .Options;
            using (var expensesDbContext = new ExpensesDbContext(_options))
            {
                expensesDbContext.Database.EnsureDeleted();
                expensesDbContext.Database.EnsureCreated();

                var expenses = GetExpenses().ToList();
                expenses.ForEach(e => expensesDbContext.Expenses.Add(e));
                expensesDbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GetExpenseByIdGetsTheCorrectExpenseItem()
        {
            var mockLogger = new Mock<ILogger<ExpensesRepository>>();
            var expenses = GetExpenses().ToList();

            using (var expensesDbContext = new ExpensesDbContext(_options))
            {
                var expenseToFind = expensesDbContext.Expenses.First();

                var repository = new ExpensesRepository(mockLogger.Object, expensesDbContext);

                var expense = await repository.GetExpenseById(expenseToFind.Id);

                Assert.Equal(expenseToFind, expense, GetExpensesComparer());
            }
        }

        [Fact]
        public async Task AddExpenseAddsTheCorrectExpenseItem()
        {
            var mockLogger = new Mock<ILogger<ExpensesRepository>>();
            var expenses = GetExpenses().ToList();

            using (var expensesDbContext = new ExpensesDbContext(_options))
            {
                var expenseToAdd = GetExpense();

                var repository = new ExpensesRepository(mockLogger.Object, expensesDbContext);

                var expense = await repository.AddExpense(expenseToAdd);

                Assert.Contains(expense, expensesDbContext.Expenses, GetExpensesComparer());
            }
        }

        [Fact]
        public async Task DeleteExpenseRemovesTheCorrectExpenseItem()
        {
            var mockLogger = new Mock<ILogger<ExpensesRepository>>();
            var expenses = GetExpenses().ToList();
            using (var expensesDbContext = new ExpensesDbContext(_options))
            {
                var expenseToDelete = expenses.First();

                var repository = new ExpensesRepository(mockLogger.Object, expensesDbContext);

                var result = await repository.DeleteExpense(expenseToDelete.Id);

                Assert.DoesNotContain(expenseToDelete, expensesDbContext.Expenses, GetExpensesComparer());
            }
        }
    }
}
