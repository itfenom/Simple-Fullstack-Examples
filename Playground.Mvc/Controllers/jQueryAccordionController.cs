using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    // ReSharper disable once InconsistentNaming
    public class jQueryAccordionController : Controller
    {
        public ActionResult Index()
        {
            return View(GetModel());
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
                    DisplayOrder = 1,
                    Skills = "N/A"
                },
                new EmployeeInfo
                {
                    EmployeeName = "Jean Doe",
                    Location = "Atlanta, GA",
                    Sex = Gender.Female,
                    IsCurrentEmployee = false,
                    DisplayOrder = 2,
                    Skills = "ASP.NET, ASP.NET MVC, C#, Visual Basic, WebForms, WinForms, WPF, Web Services"
                },
                new EmployeeInfo
                {
                    EmployeeName = "Joe Haynes",
                    Location = "New York, NY",
                    Sex = Gender.Male,
                    IsCurrentEmployee = true,
                    DisplayOrder = 3,
                    Skills = "N/A"
                },
                new EmployeeInfo
                {
                    EmployeeName = "Billy Hall",
                    Location = "New York, NY",
                    Sex = Gender.Male,
                    IsCurrentEmployee = true,
                    DisplayOrder = 4,
                    Skills = "C#, MVC, WebForms, WinForms"
                },
                new EmployeeInfo
                {
                    EmployeeName = "Daniel ?",
                    Location = "New York, NY",
                    Sex = Gender.Unknown,
                    IsCurrentEmployee = false,
                    DisplayOrder = 5,
                    Skills = "WPF"
                },
                new EmployeeInfo
                {
                    EmployeeName = "Derek ?",
                    Location = "New York, NY",
                    Sex = Gender.Unknown,
                    IsCurrentEmployee = false,
                    DisplayOrder = 6,
                    Skills = "Java"
                }
            };



            return retVal;
        }
    }
}