using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.WebApi.Extensions;
using ExpenseTracker.WebApi.Models;
using Microsoft.AspNetCore.Http;
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
        private const string SERVER_ERROR = "Internal Server Error. Something went Wrong!";
        private readonly IExpensesRepository _expensesRepository;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(ILogger<ExpensesController> logger, IExpensesRepository expensesRepository)
        {
            _logger = logger;
            _expensesRepository = expensesRepository;
        }

        /// <summary>
        /// Returns all expenses
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            try
            {
                _logger.LogDebug("ExpensesController GetExpenses invoked");
                return await _expensesRepository.Expenses.ToListAsync();
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error invoking ExpensesController GetExpenses", exp);
                return this.StatusCode(StatusCodes.Status500InternalServerError, SERVER_ERROR);
            }
        }

        /// <summary>
        /// Returns expense for the key specified
        /// </summary>
        /// <param name="key">key of the existing expense object</param>
        /// <returns></returns>
        [HttpGet("{key}")]
        public async Task<ActionResult<Expense>> GetExpense(int key)
        {
            try
            {
                _logger.LogDebug($"ExpensesController GetExpense({key}) invoked");
                var expense = await _expensesRepository.GetExpenseById(key);

                if (expense == null) return NotFound();

                return expense;
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error invoking ExpensesController GetExpense({key})", exp);
                return this.StatusCode(StatusCodes.Status500InternalServerError, SERVER_ERROR);
            }
        }

        /// <summary>
        /// The FromForm key, values to support the request mechanism used by the DevExtreme grid
        /// </summary>
        /// <param name="key">key of the existing expense object</param>
        /// <param name="values">json values for updating expense object</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateExpense([FromForm] int key, [FromForm] string values)
        {
            try
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
            catch (Exception exp)
            {
                _logger.LogError($"Error invoking ExpensesController UpdateExpense({key}, {values})", exp);
                return this.StatusCode(StatusCodes.Status500InternalServerError, SERVER_ERROR);
            }
        }

        /// <summary>
        /// The FromForm values to support the request mechanism used by the DevExtreme grid
        /// </summary>
        /// <param name="values">json values for expense object to be added</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Expense>> AddExpense([FromForm] string values)
        {
            try
            {
                _logger.LogDebug($"ExpensesController AddExpense({values}) invoked");

                var expense = new Expense();
                JsonConvert.PopulateObject(values, expense);

                if (!TryValidateModel(expense))
                    return BadRequest(ModelState.GetAllErrors());

                var created = await _expensesRepository.AddExpense(expense);

                return CreatedAtAction("GetExpense", new {key = created.Id}, created);
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error invoking ExpensesController AddExpense({values})", exp);
                return this.StatusCode(StatusCodes.Status500InternalServerError, SERVER_ERROR);
            }
        }

        /// <summary>
        /// The FromForm values to support the request mechanism used by the DevExtreme grid
        /// </summary>
        /// <param name="key">key of the expense object to delete</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteExpense([FromForm] int key)
        {
            try
            {
                _logger.LogDebug($"ExpensesController DeleteExpense({key}) invoked");

                var result = await _expensesRepository.DeleteExpense(key);

                return result ? (IActionResult) NoContent() : NotFound();
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error invoking ExpensesController DeleteExpense({key})", exp);
                return this.StatusCode(StatusCodes.Status500InternalServerError, SERVER_ERROR);
            }
        }
    }
}