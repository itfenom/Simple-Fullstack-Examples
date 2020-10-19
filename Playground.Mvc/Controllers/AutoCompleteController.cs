using Playground.Mvc.DAL;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class AutoCompleteController : BaseController
    {
        private readonly IEmployeeRepository _repository;

        public AutoCompleteController()
        {
            _repository = new EmployeeRepository(new SeraphEntities());
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = _repository.GetAllEmployees();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string searchTerm)
        {
            var model = _repository.GetEmployeesByName(searchTerm);
            return View(model);
        }

        public JsonResult GetEmployees(string term)
        {
            var model = _repository.GetEmployeesByName(term.ToLower())
                          .Select(x => x.EmpName).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}