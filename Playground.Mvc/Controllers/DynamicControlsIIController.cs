using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    // ReSharper disable once InconsistentNaming
    public class DynamicControlsIIController : BaseController
    {
        public ActionResult Index()
        {
            var model = new DynamicControls2ViewModel()
            {
                EmployeeInfo = GetModel()
            };

            return View(model);
        }

        public ActionResult ValidateSubmit(DynamicControls2ViewModel model)
        {
            return View("PostedData", model);
        }

        private List<EmployeeInfo> GetModel()
        {
            var retVal = new List<EmployeeInfo>
            {
                new EmployeeInfo
                {
                    EmployeeName = "John Doe",
                    Location = "Dallas, TX",
                    Sex = Gender.Male,
                    IsCurrentEmployee = true,
                    EditDetailsFlag = "N",
                    DisplayOrder = 1
                },
                new EmployeeInfo
                {
                    EmployeeName = "Jean Doe",
                    Location = "Atlanta, GA",
                    Sex = Gender.Female,
                    IsCurrentEmployee = false,
                    EditDetailsFlag = "N",
                    DisplayOrder = 2
                },
                new EmployeeInfo
                {
                    EmployeeName = "Joe Haynes",
                    Location = "New York, NY",
                    Sex = Gender.Male,
                    IsCurrentEmployee = true,
                    EditDetailsFlag = "N",
                    DisplayOrder = 3
                }
            };


            return retVal;
        }

        public JsonResult GetStates()
        {
            var states = new List<string>();

            states.Add("TX");
            states.Add("GA");
            states.Add("NY");
            states.Add("VA");
            states.Add("AL");

            var items = states.OrderBy(l => l).ToList()
                     .Select(i => new SelectListItem()
                     {
                         Text = i,
                         Value = i
                     }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCities(string state)
        {
            var cities = new List<string>();
            // ReSharper disable once RedundantAssignment

            if (state == "TX")
            {
                cities.Add("Arlington");
                cities.Add("Dallas");
                cities.Add("Fort Worth");
                cities.Add("El Paso");
                cities.Add("Waco");
                cities.Add("Austin");
            }
            else if (state == "GA")
            {
                cities.Add("Atlanta");
            }
            else if (state == "NY")
            {
                cities.Add("Buffalo");
                cities.Add("New York");
                cities.Add("Stoney Brook");
            }
            else if (state == "VA")
            {
                cities.Add("Arlington");
                cities.Add("Fairfax");
                cities.Add("Richmond");
            }
            else if (state == "AL")
            {
                cities.Add("Mobile");
                cities.Add("Montgomery");
                cities.Add("Troy");
                cities.Add("Huntsville");
            }

            var items = cities.OrderBy(l => l).ToList()
                .Select(i => new SelectListItem()
                {
                    Text = i,
                    Value = i
                }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSkills()
        {
            var skills = new List<string>
            {
                "C#",
                "Visual Bacis",
                "Javascript",
                "jQuery",
                "ASP.NET MVC"
            };


            var items = skills.OrderBy(l => l).ToList()
                     .Select(i => new SelectListItem()
                     {
                         Text = i,
                         Value = i
                     }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNewId()
        {
            int id = 20999;
            return Content(id.ToString());
        }
    }
}