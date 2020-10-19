using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class MultipleDropDownsController : BaseController
    {
        public ActionResult Index()
        {
            var countriesList = new List<SelectListItem>();

            countriesList.Add(new SelectListItem { Text = "Select Country", Value = "0" });
            countriesList.Add(new SelectListItem { Text = "USA", Value = "1" });
            countriesList.Add(new SelectListItem { Text = "Pakistan", Value = "2" });
            countriesList.Add(new SelectListItem { Text = "Mexico", Value = "3" });

            ViewData["countries"] = countriesList;

            return View();
        }

        public JsonResult GetStates(string id)
        {
            var stateList = new List<SelectListItem>();
            stateList.Add(new SelectListItem { Text = "Select State", Value = "0" });
            switch (id)
            {
                case "1":

                    stateList.Add(new SelectListItem { Text = "Alabama", Value = "1" });
                    stateList.Add(new SelectListItem { Text = "Texas", Value = "2" });
                    stateList.Add(new SelectListItem { Text = "New York", Value = "3" });

                    break;

                case "2":

                    stateList.Add(new SelectListItem { Text = "KPK", Value = "4" });
                    stateList.Add(new SelectListItem { Text = "Punjab", Value = "5" });

                    break;

                case "3":

                    stateList.Add(new SelectListItem { Text = "Jalisco", Value = "6" });

                    break;
            }

            return Json(new SelectList(stateList, "Value", "Text"));
        }

        public JsonResult GetCities(string id)
        {
            var citiesList = new List<SelectListItem>();
            citiesList.Add(new SelectListItem { Text = "Select City", Value = "0" });
            switch (id)
            {
                case "1": //Alabama

                    citiesList.Add(new SelectListItem { Text = "Montgomery", Value = "1" });
                    citiesList.Add(new SelectListItem { Text = "Birmingham", Value = "2" });
                    citiesList.Add(new SelectListItem { Text = "Mobile", Value = "3" });

                    break;

                case "2": //Texas

                    citiesList.Add(new SelectListItem { Text = "Austin", Value = "4" });
                    citiesList.Add(new SelectListItem { Text = "Dallas", Value = "5" });
                    citiesList.Add(new SelectListItem { Text = "Houston", Value = "6" });

                    break;

                case "3": //New York

                    citiesList.Add(new SelectListItem { Text = "New York City", Value = "7" });
                    citiesList.Add(new SelectListItem { Text = "Buffalo", Value = "8" });
                    citiesList.Add(new SelectListItem { Text = "Stoney Brook", Value = "9" });

                    break;

                case "4": //KPK

                    citiesList.Add(new SelectListItem { Text = "Peshawar", Value = "10" });
                    citiesList.Add(new SelectListItem { Text = "Bannu", Value = "11" });
                    citiesList.Add(new SelectListItem { Text = "Hangu", Value = "12" });

                    break;

                case "5": //Punjab

                    citiesList.Add(new SelectListItem { Text = "Faisalabad", Value = "13" });
                    citiesList.Add(new SelectListItem { Text = "Lahore", Value = "14" });
                    citiesList.Add(new SelectListItem { Text = "Rawalpindi", Value = "15" });

                    break;

                case "6": //Jalisco

                    citiesList.Add(new SelectListItem { Text = "Tepatitlan", Value = "16" });
                    citiesList.Add(new SelectListItem { Text = "Yukon", Value = "17" });

                    break;
            }

            return Json(new SelectList(citiesList, "Value", "Text"));
        }
    }
}