using Playground.Mvc.DAL;
using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class MvcToWebFormController : Controller
    {
        private readonly IEmployeeRepository _repository;

        public MvcToWebFormController()
        {
            _repository = new EmployeeRepository(new SeraphEntities());
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new MvcToWebFormViewModel()
            {
                Employees = GetModel()
            };

            return View(model);
        }

        private List<EmployeeViewModel> GetModel()
        {
            var retVal = new List<EmployeeViewModel>();
            var employeeObjectModel = _repository.GetAllEmployees();

            foreach (EmployeeViewModel item in employeeObjectModel)
            {
                retVal.Add(new EmployeeViewModel
                {
                    EmpID = item.EmpID,
                    EmpName = item.EmpName,
                    EmpEmail = item.EmpEmail,
                    EmpPhone = item.EmpPhone,
                    EmpSalary = item.EmpSalary,
                    EmpGender = item.EmpGender,
                    EmpHireDate = item.EmpHireDate,
                    EmpIsActive = item.EmpIsActive
                });
            }

            return retVal;
        }

        [HttpPost]
        public ActionResult Index(MvcToWebFormViewModel postedModel)
        {
            if (postedModel != null)
            {
                Session["PostedModel"] = postedModel.Employees;
                return Redirect("~/WebForms/WebFormTest.aspx");
            }

            // ReSharper disable once ExpressionIsAlwaysNull
            return View(postedModel);
        }
    }
}