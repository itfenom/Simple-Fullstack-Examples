using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class ToolTipAndPrintController : Controller
    {
        public ActionResult Index()
        {
            return View(GetModel());
        }

        private List<EmployeeInfo> GetModel()
        {
            var retVal = new List<EmployeeInfo>();

            retVal.Add(new EmployeeInfo { EmployeeName = "John Doe", Location = "Dallas, TX", Skills = "C#, Oracle, Javascript", Sex = Gender.Male, IsCurrentEmployee = true, EditDetailsFlag = "N", DisplayOrder = 1 });
            retVal.Add(new EmployeeInfo { EmployeeName = "Jean Doe", Location = "Atlanta, GA", Skills = "Database", Sex = Gender.Female, IsCurrentEmployee = false, EditDetailsFlag = "N", DisplayOrder = 2 });
            retVal.Add(new EmployeeInfo { EmployeeName = "Joe Haynes", Location = "New York, NY", Skills = "Ruby on Rails", Sex = Gender.Male, IsCurrentEmployee = true, EditDetailsFlag = "N", DisplayOrder = 3 });

            return retVal;
        }
    }
}