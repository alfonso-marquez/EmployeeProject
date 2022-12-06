using EmployeeProject.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Data
{
    public class EmployeeProjectDbContext : DbContext
    {
        public EmployeeProjectDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
