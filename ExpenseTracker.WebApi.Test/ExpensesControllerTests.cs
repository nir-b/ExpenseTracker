using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ExpenseTracker.WebApi.Controllers;
using ExpenseTracker.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ExpenseTracker.WebApi.Test
{
    public class ExpensesControllerTests
    {
        private static Expense[] GetExpenses()
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
                    Id =2,
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

        private static IEqualityComparer<Expense> GetExpensesComparer()
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

        [Fact]
        public async Task GetExpensesReturnsAllExpenses()
        {
            var data = GetExpenses();
            var mockData = data.AsQueryable().BuildMock();
            var mockLogger = new Mock<ILogger<ExpensesController>>();
            var mockRepository = new Mock<IExpensesRepository>();

            mockRepository.SetupGet(m => m.Expenses).Returns(mockData.Object);
            var controller = new ExpensesController(mockLogger.Object, mockRepository.Object);

            var response = await controller.GetExpenses();

            Assert.Equal(data, response.Value, GetExpensesComparer());

            mockRepository.VerifyGet(m => m.Expenses, Times.Once);
        }

        [Fact]
        public async Task GetExpenseReturnsExpenseByKey()
        {
            var data = GetExpenses();
            var mockLogger = new Mock<ILogger<ExpensesController>>();
            var mockRepository = new Mock<IExpensesRepository>();

            var keyToFind = data.Min(d => d.Id);
            var expenseObject = data.FirstOrDefault(d => d.Id == keyToFind);
            mockRepository.Setup(m => m.GetExpenseById(keyToFind)).Returns(Task.FromResult(expenseObject));
            var controller = new ExpensesController(mockLogger.Object, mockRepository.Object);

            
            var response = await controller.GetExpense(keyToFind);

            Assert.Equal(data.FirstOrDefault(d => d.Id == keyToFind), response.Value, GetExpensesComparer());
        }
    }
}
