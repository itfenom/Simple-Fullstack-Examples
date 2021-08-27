using Newtonsoft.Json;
using Playground.Mvc.Helpers;
using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Data;
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

        public JsonResult UpdateEmployeeWorkHistory(XyzEmployeeUpdateModel model)
        {
            var result = _repository.UpdateWorkHistory(model);
            return Json(new { isSucceed = result.IsSucceed, message = result.Message });
        }

        public FileResult GetFile(string workHistoryId)
        {
            var fileData = _repository.GetFileData(workHistoryId, out var fileType);
            return File(fileData, fileType);
        }

        #region Compelling Example 1
        public ActionResult CompellingExample1()
        {
            var model = GetSourceIds();
            return View(model);
        }

        public JsonResult GetTargetIds(string sourceId) 
        {
            try
            {
                var targetIds = GetTargetIds(Convert.ToInt32(sourceId));
                return Json(new { success = true, message = "", targetIds = targetIds }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private CompellingExample1Model GetSourceIds()
        {
            var retVal = new CompellingExample1Model();
            retVal.Ids = new List<int>();
            retVal.Ids.AddRange(new List<int> { 100, 101, 102, 103, 104 });
            return retVal;
        }

        private CompellingExample1Model GetTargetIds(int sourceId)
        {
            var retVal = new CompellingExample1Model();
            retVal.Ids = new List<int>();
            if (sourceId == 100)
            {
                retVal.Ids.AddRange(new List<int> { 2000, 2001, 2002, 2003, 2004 });
            }
            else if(sourceId == 101)
            {
                retVal.Ids.AddRange(new List<int> { 3000, 3001, 3002, 3003, 3004 });
            }
            else if (sourceId == 102)
            {
                retVal.Ids.AddRange(new List<int> { 4000, 4001, 4002, 4003, 4004 });
            }
            else if (sourceId == 103)
            {
                retVal.Ids.AddRange(new List<int> { 5000, 5001, 5002, 5003, 5004 });
            }
            else if (sourceId == 104)
            {
                retVal.Ids.AddRange(new List<int> { 6000, 6001, 6002, 6003, 6004 });
            }

            return retVal;
        }

        public JsonResult GetRouteOpersToCopy(string sourceId, List<string> targetIds)
        {
            try
            {
                var data = new CompellingExample1RouteOperModel();

                if(targetIds.Contains("4000"))
                {
                    data.FoundCommonOperations = false;
                }
                else
                {
                    data.FoundCommonOperations = true;
                    data.SourceId = sourceId;
                    data.TargetRouteOper = new List<CompellingExample1TargetModel>();

                    //REGEXP_REPLACE(LISTAGG(X.OPER, ',') WITHIN GROUP (ORDER BY X.OPER),'([^,]+)(,\1)+', '\1') AS DISTINCT_OPERS
                    //REGEXP_REPLACE(LISTAGG(X.OPER, ',') WITHIN GROUP (ORDER BY X.OPER),'([^,]+)(,\0)+', '\1') AS DISTINCT_OPERS

                    var dt = new DataTable();
                    dt.Columns.Add(new DataColumn("Id", typeof(System.Int32)));
                    dt.Columns.Add(new DataColumn("Route", typeof(System.String)));
                    dt.Columns.Add(new DataColumn("Operations", typeof(System.String)));

                    DataRow row;
                    row = dt.NewRow();
                    row["Id"] = 165383;
                    row["Route"] = "TE1B";
                    row["Operations"] = "101";
                    dt.Rows.Add(row);

                    row = dt.NewRow();
                    row["Id"] = 165282;
                    row["Route"] = "REF10";
                    row["Operations"] = "101, 106, 1511, 2281, 2284, 2295, 2306, 2436, 2437, 2502, 2608";
                    dt.Rows.Add(row);

                    row = dt.NewRow();
                    row["Id"] = 165633;
                    row["Route"] = "TRIM2";
                    row["Operations"] = "101, 105, 227, 228, 2349, 2284";
                    dt.Rows.Add(row);

                    dt.AcceptChanges();

                    foreach (DataRow r in dt.Rows)
                    {
                        var operations = new List<CompellingExample1OperationModel>();
                        var opers = r["Operations"].ToString().Split(',').ToList();
                        foreach (var item in opers)
                        {
                            operations.Add(new CompellingExample1OperationModel { Operation = item });
                        }

                        var targetSwr = new CompellingExample1TargetModel();
                        targetSwr.TargetId = r["Id"].ToString();
                        targetSwr.Route = r["Route"].ToString();
                        targetSwr.Operations = operations;

                        data.TargetRouteOper.Add(targetSwr);
                    }
                }

                return Json(new { success = true, message = "", commonRouteOpers = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CopySelectedOperations(CompellingExample1RouteOperModel model)
        {
            try
            {
                return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }

    #region CompellingExample1-related models
    public class CompellingExample1Model
    {
        public List<int> Ids { get; set; }
    }

    public class CompellingExample1RouteOperModel
    {
        public string SourceId { get; set; }
        public bool FoundCommonOperations { get; set; }
        public List<CompellingExample1TargetModel> TargetRouteOper { get; set; }

    }

    public class CompellingExample1TargetModel
    {
        public string TargetId { get; set; }
        public string Route { get; set; }

        public string RouteAndTargetId
        {
            get { return $"{TargetId} => {Route}"; }
        }

        public bool IsSelected { get; set; }

        public List<CompellingExample1OperationModel> Operations { get; set; }
    }

    public class CompellingExample1OperationModel
    {
        public string Operation { get; set; }
        public bool IsSelected { get; set; }
    }

    #endregion
}