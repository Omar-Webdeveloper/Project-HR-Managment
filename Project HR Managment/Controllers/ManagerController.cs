using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project_HR_Managment.Models;
using Rotativa.AspNetCore;

namespace Project_HR_Managment.Controllers
{
    public class ManagerController : Controller
    {
        private readonly MyDbContext _context;
        public ManagerController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult ManagerLogin()
        {
            return View();
        }
        // POST: Manager Login
        [HttpPost]
        public IActionResult ManagerLogin(Manager manager)
        {
            var existingManager = _context.Managers
                .FirstOrDefault(x => x.Email == manager.Email && x.Password == manager.Password);

            if (existingManager != null)
            {
                // Store manager details in session or TempData if needed
                HttpContext.Session.SetInt32("ManagerId", existingManager.Id);
                TempData["ManagerId"] = existingManager.Id;
                // Redirect to the dashboard or manager's homepage
                return RedirectToAction("Dashboard", new { id = existingManager.Id });
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }
        }

        public IActionResult Dashboard(int id)
        {
            var manager = _context.Managers
                .Include(m => m.Employee) // Include employees related to the manager
                .FirstOrDefault(m => m.Id == id);

            if (manager == null)
            {
                return NotFound();
            }

            // Retrieve all employees assigned to this manager
            var employees = _context.Employees.Where(e => e.ManagerId == id).ToList();

            ViewBag.ManagerName = manager.Name;
            return View(employees);
        }
        public IActionResult ViewTasks(int id)
        {
            var tasks = _context.Tasks.Where(t => t.EmployeeId == id).ToList();

            if (tasks == null || tasks.Count == 0)
            {
                ViewBag.Message = "No tasks assigned to this employee.";
            }

            return View(tasks);
        }
        public IActionResult ViewAttendance(int id)
        {
            var attendanceRecords = _context.Attendances.Where(a => a.EmployeeId == id).ToList();

            if (attendanceRecords == null || attendanceRecords.Count == 0)
            {
                ViewBag.Message = "No attendance records found for this employee.";
            }

            return View(attendanceRecords);
        }
        public IActionResult ExportTasksAsPdf(int id)
        {
            var tasks = _context.Tasks.Where(t => t.EmployeeId == id).ToList();

            if (tasks == null || tasks.Count == 0)
            {
                ViewBag.Message = "No tasks assigned to this employee.";
                return View("ViewTasks", tasks);
            }

            return new ViewAsPdf("ExportTasksAsPdf", tasks)
            {
                FileName = "EmployeeTasks.pdf"
            };
        }
        public IActionResult ExportAttendanceAsPdf(int id)
        {
            var attendance = _context.Attendances.Where(a => a.EmployeeId == id).ToList();

            if (attendance == null || attendance.Count == 0)
            {
                ViewBag.Message = "No attendance records found for this employee.";
                return View("ViewAttendance", attendance);
            }

            return new ViewAsPdf("ExportAttendanceAsPdf", attendance)
            {
                FileName = "EmployeeAttendance.pdf"
            };
        }
        //public IActionResult Employee_evaluation( )
        //{
        //    return View();

        //}
        public IActionResult Employee_Evaluation(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.EmployeeId = id;
            ViewBag.ManagerId = HttpContext.Session.GetInt32("ManagerId");
            ViewBag.Questions = new List<string>
    {
        "How well does the employee complete tasks on time?",
        "How effective is the employee’s communication?",
        "How well does the employee work in a team?",
        "How proactive is the employee?",
        "How innovative is the employee?",
        "How well does the employee handle stress?",
        "How committed is the employee to the company’s goals?",
        "How well does the employee follow instructions?",
        "How punctual is the employee?",
        "How adaptable is the employee to new situations?"
    };

            return View();
        }
        [HttpPost]
        public IActionResult Submit_Evaluation(int EmployeeId, int ManagerId, List<int> Answers, string Comments)
        {
            if (Answers == null || Answers.Count < 10)
            {
                ModelState.AddModelError("", "Please answer all questions.");
                return RedirectToAction("Employee_Evaluation", new { id = EmployeeId });
            }

            // Calculate total score
            int totalScore = Answers.Sum();

            // Determine Status based on score
            string status;
            if (totalScore >= 40) status = "Excellent";
            else if (totalScore >= 30) status = "Good";
            else if (totalScore >= 20) status = "Average";
            else status = "Needs Improvement";

            // Save evaluation to the database
            var evaluation = new Evaluation
            {
                EmployeeId = EmployeeId,
                ManegerId = ManagerId,
                EvaluationDate = DateOnly.FromDateTime(DateTime.Now),
                Status = status
            };

            _context.Evaluations.Add(evaluation);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", new { id = ManagerId });
        }

        //public IActionResult ExportEmployeesAsPdf(int managerId)
        //{
        //    var employees = _context.Employees
        //        .Where(e => e.ManagerId == managerId)
        //        .Select(e => new
        //        {
        //            e.Name,
        //            e.Email,
        //            e.Position,
        //            e.ProfileImage,
        //            DepartmentName = e.Department != null ? e.Department.Name : "N/A"
        //        }).ToList();

        //    using (var stream = new MemoryStream())
        //    {
        //        PdfWriter writer = new PdfWriter(stream);
        //        PdfDocument pdf = new PdfDocument(writer);
        //        Document document = new Document(pdf);

        //        document.Add(new Paragraph("Manager's Employees Report").SetBold().SetFontSize(18));
        //        document.Add(new Paragraph($"Generated on: {DateTime.Now}\n\n"));

        //        Table table = new Table(4);
        //        table.AddHeaderCell("Department");
        //        table.AddHeaderCell("Employee Name");
        //        table.AddHeaderCell("Position");
        //        table.AddHeaderCell("Email");

        //        foreach (var employee in employees)
        //        {
        //            table.AddCell(employee.DepartmentName);
        //            table.AddCell(employee.Name ?? "N/A");
        //            table.AddCell(employee.Position ?? "N/A");
        //            table.AddCell(employee.Email ?? "N/A");
        //        }

        //        document.Add(table);
        //        document.Close();

        //        var pdfBytes = stream.ToArray();
        //        return File(pdfBytes, "application/pdf", "ManagerEmployees.pdf");
        //    }


        //public IActionResult ExportEmployeesAsPdf(int id)
        //{
        //    var employees = _context.Employees
        //        .Where(e => e.ManagerId == id)
        //        .Select(e => new EmployeeViewModel
        //        {
        //            Name = e.Name,
        //            Email = e.Email,
        //            Position = e.Position,
        //            ProfileImage = e.ProfileImage
        //        })
        //        .ToList();

        //    if (employees == null || employees.Count == 0)
        //    {
        //        ViewBag.Message = "No employees found for this manager.";
        //        return View("ViewEmployees", employees);
        //    }

        //    return new ViewAsPdf("ExportEmployeesAsPdf", employees)
        //    {
        //        FileName = "Manager_Employees.pdf"
        //    };
        //}












        public IActionResult ExportEmployeesAsPdf(int id)
        {
            var employees = _context.Employees
                .Where(e => e.ManagerId == id)
                .Select(e => new
                {
                    e.Name,
                    e.Email,
                    e.Position,
                    e.ProfileImage
                })
                .ToList();

            //if (employees == null || employees.Count == 0)
            //{
            //    ViewBag.Message = "No employees found for this manager.";
            //    return View("ViewEmployees", employees);
            //}

            return new ViewAsPdf("ExportEmployeesAsPdf", employees)
            {
                FileName = "Manager_Employees.pdf"
            };
        }
    }
}
