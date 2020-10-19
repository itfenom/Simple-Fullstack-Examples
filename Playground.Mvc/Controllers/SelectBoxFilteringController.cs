using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class SelectBoxFilteringController : Controller
    {
        private List<SelectListItem> _data;

        public SelectBoxFilteringController()
        {
            _data = GetDataToDisplay();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (_data == null || _data.Count == 0)
            {
                _data = GetDataToDisplay();
            }

            ViewData["Items"] = _data;
            return View();
        }

        public ActionResult FilterByAjaxCall(string searchStr)
        {
            if (_data == null || _data.Count == 0)
            {
                _data = GetDataToDisplay();
            }

            if (string.IsNullOrEmpty(searchStr))
            {
                return Json(_data, JsonRequestBehavior.AllowGet);
            }

            var filteredItems = new List<SelectListItem>(_data.Where(c => c.Text.ToString().ToLower().Contains(searchStr.ToLower()))
                .Select(c => new SelectListItem
                {
                    Value = c.Value,
                    Text = c.Text
                }));

            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetDataToDisplay()
        {
            var retVal = new List<SelectListItem>();

            retVal.Add(new SelectListItem { Text = "12245(Test1:START-111)", Value = "12245" });
            retVal.Add(new SelectListItem { Text = "12246(Test2:START-111)", Value = "12246" });
            retVal.Add(new SelectListItem { Text = "12247(Test3:START-111)", Value = "12247" });
            retVal.Add(new SelectListItem { Text = "12248(Test4:START-111)", Value = "12248" });
            retVal.Add(new SelectListItem { Text = "12249(Test5:STOP-111)", Value = "12249" });
            retVal.Add(new SelectListItem { Text = "12250(Test6:STOP-111)", Value = "12250" });
            retVal.Add(new SelectListItem { Text = "12251(Test7:STOP-111)", Value = "12251" });
            retVal.Add(new SelectListItem { Text = "12252(Test8:STOP-111)", Value = "12252" });
            retVal.Add(new SelectListItem { Text = "12253(Test9:STOP-111)", Value = "12252" });

            return retVal;
        }
    }
}