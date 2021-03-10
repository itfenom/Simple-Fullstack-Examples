using Playground.Mvc.DAL;
using Playground.Mvc.Helpers;
using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    // ReSharper disable once IdentifierTypo
    public class FlexiGridEmployeeMgrController : BaseController
    {
        private readonly IEmployeeRepository _repository;

        // ReSharper disable once IdentifierTypo
        public FlexiGridEmployeeMgrController()
        {
            _repository = new EmployeeRepository(new SeraphEntities());
        }

        public ActionResult Index()
        {
            ViewBag.BasePath = Url.Content("~").WithTrailingSlash();
            return View();
        }

        public ActionResult EmployeesList(int page, int rp, string sortName, string sortOrder, string qType, string query)
        {
            var allEmployees = _repository.GetAllEmployees().AsQueryable();

            var total = allEmployees.Count();

            if (!string.IsNullOrEmpty(qType) && !string.IsNullOrEmpty(query))
            {
                allEmployees = allEmployees.Like(qType, query);
            }

            if (!string.IsNullOrEmpty(sortName) && !string.IsNullOrEmpty(sortOrder))
            {
                allEmployees = allEmployees.OrderBy(sortName, sortOrder == "asc");
            }

            allEmployees = allEmployees.Skip((page - 1) * rp).Take(rp);

            return CreateFlexiJson(allEmployees.ToList(), page, total);
        }

        [HttpPost]
        public ActionResult SaveEmployee(EmployeeViewModel employeeObj)
        {
            if (employeeObj.EmpID == 0) // if ID == 0, its a new entry
            {
                _repository.AddNewEmployee(employeeObj);
            }
            else if (employeeObj.EmpID > 0) //if ID field is not 0, it is an update!!!
            {
                employeeObj.File = null;
                employeeObj.EmpPhoto = null;
                employeeObj.EmpPhotoId = null;
                _repository.UpdateEmployee(employeeObj);
            }

            var employees = _repository.GetAllEmployees().OrderByDescending(x => x.EmpID).AsQueryable();
            return CreateFlexiJson(employees.ToList(), 1, employees.Count());
        }

        [HttpPost]
        public ActionResult DeleteEmployee(int employeeId)
        {
            if (_repository.DeleteEmployee(employeeId))
            {
                ViewBag.Message = "Employee Deleted successfully!";
            }

            var employees = _repository.GetAllEmployees().OrderByDescending(x => x.EmpID).AsQueryable();
            return CreateFlexiJson(employees.ToList(), 1, employees.Count());
        }

        // ReSharper disable once IdentifierTypo
        private JsonResult CreateFlexiJson(IEnumerable<EmployeeViewModel> items, int page, int total)
        {
            return Json(new
            {
                page,
                total,
                rows = items.Select(x => new
                {
                    id = x.EmpID,
                    cell = new object[]
                    {    // The Cell values needs to be in the same order that they supposed to appear on the FlexiGrid!
                        x.EmpID,
                        x.EmpName,
                        x.EmpEmail,
                        x.EmpPhone,
                        x.HireDate,
                        x.IsActive,
                        x.Salary,
                        x.EmpGender
                    }
                })
            }, JsonRequestBehavior.AllowGet);
        }
    }
}