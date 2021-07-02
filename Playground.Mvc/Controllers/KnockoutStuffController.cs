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
    }
}