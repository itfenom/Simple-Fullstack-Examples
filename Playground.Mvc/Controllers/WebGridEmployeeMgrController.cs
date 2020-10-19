using Playground.Mvc.DAL;
using Playground.Mvc.Helpers;
using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class WebGridEmployeeMgrController : BaseController
    {
        private readonly IEmployeeRepository _repository;

        public WebGridEmployeeMgrController()
        {
            _repository = new EmployeeRepository(new SeraphEntities());
        }

        public ActionResult Index(int page = 1, string sort = "EmpName", string sortDir = "ASC", string searchTerm = null, string filterBy = null)
        {
            const int maxPages = 10;
            var totalRows = GetCount(searchTerm, filterBy);
            var dir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase);
            var employees = GetEmployees(page, maxPages, sort, dir, searchTerm, filterBy);

            var model = new WebGridEmployeeViewModel()
            {
                Pages = maxPages,
                Rows = totalRows,
                Employees = employees
            };
            return View(model);
        }

        private int GetCount(string searchTerm, string filterBy)
        {
            int retVal;

            if (!string.IsNullOrEmpty(filterBy))
            {
                if (filterBy.ToUpper() == "EMP_NAME")
                {
                    retVal = _repository.GetEmployeesByName(searchTerm).Count();
                }
                else if (filterBy.ToUpper() == "EMP_EMAIL")
                {
                    retVal = _repository.GetEmployeesByEmail(searchTerm).Count();
                }
                else
                {
                    retVal = _repository.GetAllEmployees().Count();
                }
            }
            else
            {
                retVal = _repository.GetAllEmployees().Count();
            }

            return retVal;
        }

        private IEnumerable<EmployeeViewModel> GetEmployees(int pageNumber, int pageSize, string sort, bool dir, string searchTerm, string filterBy)
        {
            var e = _repository.GetAllEmployees();

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (!string.IsNullOrEmpty(filterBy))
            {
                if (filterBy == "EMP_NAME")
                {
                    e = _repository.GetEmployeesByName(searchTerm);
                }
                else if (filterBy == "EMP_EMAIL")
                {
                    e = _repository.GetEmployeesByEmail(searchTerm);
                }
            }

            if (sort == "EmpName")
            {
                return e.OrderByWithDirection(x => x.EmpName, dir)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            if (sort == "EmpEmail")
            {
                return e.OrderByWithDirection(x => x.EmpEmail, dir)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            if (sort == "Salary")
            {
                return e.OrderByWithDirection(x => x.Salary, dir)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            else if (sort == "HireDate")
            {
                return e.OrderByWithDirection(x => x.HireDate, dir)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            return e;
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EmployeeViewModel newEmployeeModel)
        {
            if (ModelState.IsValid)
            {
                if (newEmployeeModel.EmpPhone.Length > 0)
                {
                    string phoneNumberPattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                    if (!Regex.IsMatch(newEmployeeModel.EmpPhone, phoneNumberPattern))
                    {
                        ModelState.AddModelError("", "Invalid Phone number entered!");
                    }
                    else
                    {
                        _repository.AddNewEmployee(newEmployeeModel);
                        Success($"<b>{"SUCCESS"}</b> New Employee Added succcessfully!.", true);
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Phone number entered!");
                }
            }

            return View(newEmployeeModel);
        }

        [HttpGet]
        public ActionResult Edit(int empId)
        {
            var model = _repository.GetEmployeeById(empId);

            ViewBag.EmployeeStatus = GetActiveStatusList(model.IsActive);

            return View(model);
        }

        private List<SelectListItem> GetActiveStatusList(string status)
        {
            var selectedListItems = new List<SelectListItem>();
            if (status == "Yes")
            {
                var selectedItem = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Yes",
                    Selected = true
                };
                selectedListItems.Add(selectedItem);

                var unselectedItem = new SelectListItem
                {
                    Text = "No",
                    Value = "No",
                    Selected = false
                };

                selectedListItems.Add(unselectedItem);
            }
            else if (status == "No")
            {
                var selectedItem = new SelectListItem
                {
                    Text = "No",
                    Value = "No",
                    Selected = true
                };
                selectedListItems.Add(selectedItem);

                var unselectedItem = new SelectListItem
                {
                    Text = "Yes",
                    Value = "Yes",
                    Selected = false
                };

                selectedListItems.Add(unselectedItem);
            }

            return selectedListItems;
        }

        [HttpPost]
        public ActionResult Edit(EmployeeViewModel updatedEmployeeModel)
        {
            ViewBag.EmployeeStatus = GetActiveStatusList(updatedEmployeeModel.EmpStatus);

            if (ModelState.IsValid)
            {
                if (updatedEmployeeModel.EmpPhone.Length > 0)
                {
                    string phoneNumberPattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                    if (!Regex.IsMatch(updatedEmployeeModel.EmpPhone, phoneNumberPattern))
                    {
                        ModelState.AddModelError("", "Invalid Phone number entered!");
                    }
                    else
                    {
                        // ReSharper disable once SimplifyConditionalTernaryExpression
                        updatedEmployeeModel.EmpIsActive = (updatedEmployeeModel.EmpStatus == "No" ? false : true);

                        _repository.UpdateEmployee(updatedEmployeeModel);

                        Success($"<b>{"SUCCESS"}</b> Employee Edited succcessfully!.", true);

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Phone number entered!");
                }
            }

            return View(updatedEmployeeModel);
        }

        public ActionResult Delete(int empId)
        {
            _repository.DeleteEmployee(empId);

            Success($"<b>{"SUCCESS"}</b> Employee Deleted succcessfully!.", true);

            return RedirectToAction("Index");
        }

        public ActionResult ExportToExcel()
        {
            var model = _repository.GetAllEmployees();
            var grid = new System.Web.UI.WebControls.GridView();

            var fileName = "Employees_Exported_Data_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ".xls";
            grid.DataSource = model;
            grid.DataBind();

            return new DownloadExcelFileActionResult(grid, fileName);
        }
    }
}