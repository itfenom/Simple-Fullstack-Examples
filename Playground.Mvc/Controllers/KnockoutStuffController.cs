using Newtonsoft.Json;
using Playground.Mvc.Helpers;
using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class KnockoutStuffController : Controller
    {
        private readonly XyzEmployeeRepository _repository = new XyzEmployeeRepository();

        public ActionResult Index()
        {
            ViewBag.BasePath = Url.Content("~").WithTrailingSlash();
            return View();
        }

        public JsonResult GetEmployees()
        {
            var queryString = Request.QueryString.AllKeys.ToDictionary(x => x, x => Request.QueryString[x]);
            var request = new JqGridRequest();
            request.Page = Convert.ToInt32(queryString["page"]);
            request.PerPage = Convert.ToInt32(queryString["rows"]);
            request.SortByProperty = queryString["sidx"];

            if(queryString["sord"] == "asc")
            {
                request.SortOrder = SortOrder.Ascending;
            }
            else
            {
                request.SortOrder = SortOrder.Descending;
            }

            bool doSearch = queryString["_search"] == "true";
            if(doSearch)
            {
                queryString.Remove("page");
                queryString.Remove("rows");
                queryString.Remove("sord");
                queryString.Remove("nd");
                queryString.Remove("_search");
                queryString.Remove("sidx");

                request.SearchPropertiesAndTerms = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>();
                if (queryString.ContainsKey("filters"))
                {
                    dynamic filters = JsonConvert.DeserializeObject(queryString["filters"]);
                    foreach (var rule in filters["rules"])
                    {
                        request.SearchPropertiesAndTerms.Add(new KeyValuePair<string, string>(rule["field"].ToString(), rule["data"].ToString()));
                    }

                    queryString.Remove("filters");
                }

                foreach (var kvp in queryString)
                {
                    request.SearchPropertiesAndTerms.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value));
                }
            }

            var model = _repository.GetEmployeeFilteredList(request);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistinctColumnValues(string columnName)
        {
            var values = _repository.GetDistinctColumnValues(columnName);
            return Json(values, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageEmployees()
        {
            var model = _repository.GetEmployees();
            return View(model);
        }

        public JsonResult SaveNewEmployee(string name, string company, string email, string gender, string salary, string title)
        {
            var result = _repository.InsertNewEmployee(name, company, email, gender, salary, title);
            return Json(new { isSucceed = result.IsSucceed, message = result.Message });
        }

        public JsonResult SaveWorkHistory(string empId, string company, string title, string salary, string skills, string hobbies, string status, string hireDate)
        {
            var result = _repository.InsertWorkHistory(empId, company, title, salary, skills, hobbies, status, hireDate);
            return Json(new { isSucceed = result.IsSucceed, message = result.Message });
        }

        public JsonResult GetWorkHistoryDataToEdit(string employeeId, string workHistoryId)
        {
            var dt = _repository.GetWorkHistoryDataTable(employeeId, workHistoryId);
            var hireDate = Convert.ToDateTime(dt.Rows[0]["HIRE_DATE"]).ToShortDateString();
            var skills = new List<string>();
            var hobbies = new List<string>();

            if(dt.Rows[0]["SKILLS"] != DBNull.Value)
            {
                skills = dt.Rows[0]["SKILLS"].ToString().Split(',').ToList().Select(x => x.Trim()).ToList();
            }

            if (dt.Rows[0]["HOBBIES"] != DBNull.Value)
            {
                hobbies = dt.Rows[0]["HOBBIES"].ToString().Split(',').ToList().Select(x => x.Trim()).ToList();
            }

            return Json(
                new
                {
                    Company = dt.Rows[0]["COMPANY_NAME"].ToString(),
                    Title = dt.Rows[0]["TITLE"].ToString(),
                    HireDate = hireDate,
                    Status = dt.Rows[0]["STATUS"].ToString(),
                    Salary = Convert.ToDecimal(dt.Rows[0]["SALARY"]),
                    Skills = skills,
                    Hobbies = hobbies,
                    FileName = dt.Rows[0]["FILE_NAME"].ToString(),
                    FileType = dt.Rows[0]["FILE_TYPE"].ToString(),
                    isSucceed = true,
                    message = ""
                }, JsonRequestBehavior.AllowGet);
        }

        /*
         * string empId, string wrkHistId, string company, string status, string title, string salary, string skills, string hobbies, string hireDate, byte[] selectedDocumentationFileData, string fileName
        XyzEmployeeUpdateModel model
         */

        public JsonResult UpdateEmployeeWorkHistory(XyzEmployeeUpdateModel model)
        {
            var result = new Result();
            result.IsSucceed = true;
            result.Message = "";

            return Json(new { isSucceed = result.IsSucceed, message = result.Message });
        }
    }
}