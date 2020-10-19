using System.Threading;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class DisablePageOnSubmitController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostedData(string value)
        {
            ViewData["POSTED_LINK_TEXT"] = "No Value!";

            Thread.Sleep(6000); //sleep for 6 seconds to show the animation to user

            if (!string.IsNullOrEmpty(value))
            {
                ViewData["POSTED_LINK_TEXT"] = value;
            }

            return View("Ok", ViewData["POSTED_LINK_TEXT"]);
        }
    }
}