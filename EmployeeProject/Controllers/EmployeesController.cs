using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var employee = await employeeProjectDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    Department = employee.Department,
                    DateofBirth = employee.DateofBirth
                };

                return await Task.Run(() => View("View", viewModel));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel model)
        {
            var employee = await employeeProjectDbContext.Employees.FindAsync(model.Id);

            if (employee != null)
            {
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Salary = model.Salary;
                employee.DateofBirth = model.DateofBirth;
                employee.Department = model.Department;

                await employeeProjectDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
                //error page
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel model)
        {
            var employee = await employeeProjectDbContext.Employees.FindAsync(model.Id);

            if (employee != null)
            {
                employeeProjectDbContext.Employees.Remove(employee);
                await employeeProjectDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult>  Excel()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");
            var currentRow = 1;
            var employees = await employeeProjectDbContext.Employees.ToListAsync();

            worksheet.Cell(currentRow, 1).Value = "Id";
            worksheet.Cell(currentRow, 2).Value = "Name";
            worksheet.Cell(currentRow, 3).Value = "Email";
            worksheet.Cell(currentRow, 4).Value = "Salary";
            worksheet.Cell(currentRow, 5).Value = "DateofBirth";
            worksheet.Cell(currentRow, 6).Value = "Department";

            foreach (var employee in employees)
            {
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = employee.Id;
                worksheet.Cell(currentRow, 2).Value = employee.Name;
                worksheet.Cell(currentRow, 3).Value = employee.Email;
                worksheet.Cell(currentRow, 4).Value = employee.Salary;
                worksheet.Cell(currentRow, 5).Value = employee.DateofBirth.ToShortDateString();
                worksheet.Cell(currentRow, 5).Value = employee.Department;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "employees.xlsx");
        }

    }
}
