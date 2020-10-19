using Playground.Mvc.DAL;
using Playground.Mvc.Models;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class EmployeesWithPhotoMgrController : BaseController
    {
        private readonly IEmployeeRepository _repository;

        public EmployeesWithPhotoMgrController()
        {
            _repository = new EmployeeRepository(new SeraphEntities());
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = _repository.GetSampleEmployeeData();

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        public ActionResult Details(int employeeId)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            /*
                if (employeeId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            */

            var model = _repository.GetEmployeeById(employeeId);
            return PartialView("_Details", model);
        }

        public ActionResult ShowEmployeePhoto(int employeeId)
        {
            var empPhoto = _repository.GetEmpPhoto(employeeId);

            return File(empPhoto, "image/jpeg");
        }

        public ActionResult Delete(int employeeId)
        {
            _repository.DeleteEmployee(employeeId);
            Success($"<b>{"SUCCESS"}</b> New Employee Deleted successfully!.", true);
            return RedirectToAction("Index");
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
                if (newEmployeeModel.File != null)
                {
                    int maxFileSize = 1024 * 1024 * 2; //2MB
                    string[] allowedFileExtensions = new string[] { ".jpg", ".png", ".gif" };
                    var attachedFile = newEmployeeModel.File;
                    var fileSize = attachedFile.ContentLength;
                    if (fileSize > maxFileSize)
                    {
                        ModelState.AddModelError("File", "File is greater than 2MB. Choose a smaller file and try again.");
                    }
                    else
                    {
                        if (!allowedFileExtensions.Contains(newEmployeeModel.File.FileName.Substring(newEmployeeModel.File.FileName.LastIndexOf('.'))))
                        {
                            ModelState.AddModelError("File", "Allowed file types are: " + string.Join(", ", allowedFileExtensions));
                        }
                        else
                        {
                            if (_repository.AddNewEmployee(newEmployeeModel))
                            {
                                Success($"<b>{"SUCCESS"}</b> New Employee Added successfully!.", true);
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Employee photo is required!");
                    return View(newEmployeeModel);
                }
            }

            return View(newEmployeeModel);
        }
    }
}