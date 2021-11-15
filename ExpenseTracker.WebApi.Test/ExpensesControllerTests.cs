using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ExpenseTracker.WebApi.Controllers;
using ExpenseTracker.WebApi.Models;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ExpenseTracker.WebApi.Test
{
    public class ExpensesControllerTests : TestBase
    {
        [Fact]
        public async Task AddExpenseAddsTheCorrectExpenseInstance()
        {
            var expense = GetExpense();
            var mockLogger = new Mock<ILogger<ExpensesController>>();
            var mockRepository = new Mock<IExpensesRepository>();
            mockRepository.Setup(r => r.AddExpense(It.IsAny<Expense>())).Returns(Task.FromResult(expense));

            var controller = new ExpensesController(mockLogger.Object, mockRepository.Object).SetupController();
            var response = await controller.AddExpense(JsonSerializer.Serialize(expense));

            mockRepository.Verify(r => r.AddExpense(It.Is(expense, GetExpensesComparer())), Times.Once);
        }

        [Fact]
        public async Task DeleteExpenseDeletesExpense()
        {
            var data = GetExpenses();
            var mockLogger = new Mock<ILogger<ExpensesController>>();
            var mockRepository = new Mock<IExpensesRepository>();
            var expenseToDelete = data.First();
           
            var controller = new ExpensesController(mockLogger.Object, mockRepository.Object).SetupController();
            var response = await controller.DeleteExpense(expenseToDelete.Id);

            mockRepository.Verify(r => r.DeleteExpense(It.Is<int>(id => id == expenseToDelete.Id)), Times.Once);
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

            var controller = new ExpensesController(mockLogger.Object, mockRepository.Object).SetupController();
            var response = await controller.GetExpense(keyToFind);

            Assert.Equal(data.FirstOrDefault(d => d.Id == keyToFind), response.Value, GetExpensesComparer());
        }

        [Fact]
        public async Task GetExpensesReturnsAllExpenses()
        {
            var data = GetExpenses();
            var mockData = data.AsQueryable().BuildMock();
            var mockLogger = new Mock<ILogger<ExpensesController>>();
            var mockRepository = new Mock<IExpensesRepository>();
            mockRepository.SetupGet(m => m.Expenses).Returns(mockData.Object);

            var controller = new ExpensesController(mockLogger.Object, mockRepository.Object).SetupController();
            var response = await controller.GetExpenses();

            Assert.Equal(data, response.Value, GetExpensesComparer());
            mockRepository.VerifyGet(m => m.Expenses, Times.Once);
        }

        [Fact]
        public async Task UpdateExpenseUpdatesExpense()
        {
            var data = GetExpenses();
            var mockLogger = new Mock<ILogger<ExpensesController>>();
            var mockRepository = new Mock<IExpensesRepository>();
            var updatedExpense = GetExpense();

            var expenseToUpdate = data.First();
            mockRepository.Setup(m => m.GetExpenseById(expenseToUpdate.Id)).Returns(Task.FromResult(expenseToUpdate));
            mockRepository.Setup(r => r.UpdateExpense(It.IsAny<Expense>())).Returns(Task.FromResult(true));

            var controller = new ExpensesController(mockLogger.Object, mockRepository.Object).SetupController();
            var response = await controller.UpdateExpense(expenseToUpdate.Id, JsonSerializer.Serialize(updatedExpense));

            mockRepository.Verify(r => r.UpdateExpense(It.Is(updatedExpense, GetExpensesComparer())), Times.Once);
        }
    }
}