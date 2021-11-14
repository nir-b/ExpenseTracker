using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.WebApi.Models
{
    public class Expense
    {
        [Key] public int Id { get; set; }

        [StringLength(512)] public string Title { get; set; }

        [StringLength(128)] public string Category { get; set; }

        public DateTime ExpenseDate { get; set; }

        [Column(TypeName = "decimal(8, 2)")] public double Amount { get; set; }

        [StringLength(1024)] public string Description { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}