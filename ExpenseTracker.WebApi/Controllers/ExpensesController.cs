using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.WebApi.Extensions;
using ExpenseTracker.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ExpenseTracker.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpensesRepository _expensesRepository;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(ILogger<ExpensesController> logger, IExpensesRepository expensesRepository)
        {
            _logger = logger;
            _expensesRepository = expensesRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            _logger.LogDebug("ExpensesController GetExpenses invoked");
            return await _expensesRepository.Expenses.ToListAsync();
        }


        [HttpGet("{key}")]
        public async Task<ActionResult<Expense>> GetExpense(int key)
        {
            _logger.LogDebug($"ExpensesController GetExpense({key}) invoked");
            var expense = await _expensesRepository.GetExpenseById(key);

            if (expense == null) return NotFound();

            return expense;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExpense([FromForm] int key, [FromForm] string values)
        {
            _logger.LogDebug($"ExpensesController UpdateExpense({key}, {values}) invoked");

            var expense = await _expensesRepository.GetExpenseById(key);
            if (expense == null) return BadRequest();

            JsonConvert.PopulateObject(values, expense);

            if (!TryValidateModel(expense))
                return BadRequest(ModelState.GetAllErrors());

            var result = await _expensesRepository.UpdateExpense(expense);

            return result ? (IActionResult) NoContent() : NotFound();
        }


        [HttpPost]
        public async Task<ActionResult<Expense>> AddExpense([FromForm] string values)
        {
            _logger.LogDebug($"ExpensesController AddExpense({values}) invoked");

            var expense = new Expense();
            JsonConvert.PopulateObject(values, expense);

            if (!TryValidateModel(expense))
                return BadRequest(ModelState.GetAllErrors());

            var created = await _expensesRepository.AddExpense(expense);

            return CreatedAtAction("GetExpense", new {key = created.Id}, created);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteExpense([FromForm] int key)
        {
            _logger.LogDebug($"ExpensesController DeleteExpense({key}) invoked");

            var result = await _expensesRepository.DeleteExpense(key);

            return result ? (IActionResult) NoContent() : NotFound();
        }
    }
}