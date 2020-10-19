using Playground.Mvc.DAL;
using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class MultiSelectListBoxController : BaseController
    {
        private readonly IEmployeeRepository _repository;

        public MultiSelectListBoxController()
        {
            _repository = new EmployeeRepository(new SeraphEntities());
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(GetSelectedList());
        }

        [HttpPost]
        public ActionResult Index(IEnumerable<string> selectedEmployees)
        {
            var model = GetSelectedList();

            if (selectedEmployees != null)
            {
                var sb = new StringBuilder();
                sb.Append("You selected employee with ID(s): ");

                foreach (string s in selectedEmployees)
                {
                    sb.Append(s + ", ");
                }

                ViewBag.SelectedEmployees = sb.Remove(sb.ToString().LastIndexOf(','), 1);
            }

            return View(model);
        }

        private EmployeeMultiSelectListViewModel GetSelectedList()
        {
            var model = new EmployeeMultiSelectListViewModel();
            var employees = _repository.GetAllEmployees();

            var l = new SelectList(employees, "EmpID", "EmpName", 0);
            model.EmployeesList = l;

            return model;
        }
    }
}