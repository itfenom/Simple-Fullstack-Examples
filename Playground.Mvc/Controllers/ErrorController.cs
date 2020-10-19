using Playground.Mvc.Models;
using System;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(int statusCode, Exception exception, bool isAjaxRequest)
        {
            Response.StatusCode = statusCode;

            // If it's not an AJAX request that triggered this action then just retun the view
            if (!isAjaxRequest)
            {
                ErrorModel model = new ErrorModel { HttpStatusCode = statusCode, Exception = exception };

                return View(model);
            }

            // Otherwise, if it was an AJAX request, return an anon type with the message from the exception
            var errorObject = new { message = exception.Message };
            return Json(errorObject, JsonRequestBehavior.AllowGet);
        }
    }
}