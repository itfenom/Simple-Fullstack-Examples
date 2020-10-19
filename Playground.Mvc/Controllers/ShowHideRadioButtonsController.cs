using System.Text;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class ShowHideRadioButtonsController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.UserAnswer = null;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string userAnswer)
        {
            if (!string.IsNullOrEmpty(userAnswer))
            {
                var parts = userAnswer.Split('|');
                var sb = new StringBuilder();

                sb.Append("<strong>Your responses:</strong><br />");

                if (parts.Length == 1)
                {
                    sb.Append(parts[0] + "<br />");
                }
                else if (parts.Length == 2)
                {
                    sb.Append(parts[0] + "<br />" + parts[1] + "<br />");
                }
                else if (parts.Length == 3)
                {
                    sb.Append(parts[0] + "<br />" + parts[1] + "<br />" + parts[2] + "<br />");
                }

                ViewBag.UserAnswer = sb;

                return View();
            }

            return View();
        }
    }
}