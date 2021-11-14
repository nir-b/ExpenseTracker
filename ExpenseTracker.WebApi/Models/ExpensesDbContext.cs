using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.WebApi.Models
{
    public class ExpensesDbContext : DbContext
    {
        public ExpensesDbContext(DbContextOptions<ExpensesDbContext> options) : base(options)
        {
        }


        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Support soft delete, using a global query filter
            modelBuilder.Entity<Expense>().HasQueryFilter(e => !e.DateDeleted.HasValue);
        }
    }
}