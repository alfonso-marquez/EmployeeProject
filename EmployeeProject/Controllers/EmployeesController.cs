using EmployeeProject.Data;
using EmployeeProject.Models;
using EmployeeProject.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeProjectDbContext employeeProjectDbContext;

        public EmployeesController(EmployeeProjectDbContext employeeProjectDbContext)
        {
            this.employeeProjectDbContext = employeeProjectDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await employeeProjectDbContext.Employees.ToListAsync();
            return View(employees);
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]

         public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                Department = addEmployeeRequest.Department,
                DateofBirth = addEmployeeRequest.DateofBirth
            };

            await employeeProjectDbContext.Employees.AddAsync(employee);
            await employeeProjectDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
} 
