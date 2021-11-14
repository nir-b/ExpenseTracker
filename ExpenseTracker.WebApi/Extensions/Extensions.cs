using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExpenseTracker.WebApi.Extensions
{
    public static class Extensions
    {
        public static string GetAllErrors(this ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            foreach (var error in entry.Value.Errors)
                messages.Add(error.ErrorMessage);

            return string.Join(" ", messages);
        }
    }
}