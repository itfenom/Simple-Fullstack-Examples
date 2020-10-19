using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Playground.Mvc.Helpers
{
    //Decorate this class attribute to the controller which requires additional authentication.
    public class OverrideAuthorizeClass : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "RestrictedControllerName", action = "PageToRedirectIfNotAuthorized<AccessDenied>" }));
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool retVal = false;

            /*************************************************
             * This is where the logic goes that determines if the user was authorized to view this page!!!
             *
                IMyRepository _repository = new MyRepository(new DataContext());
                string _userName = httpContext.User.Identity.Name.ToLower().Replace("corp", "").Replace("tqs", "").Replace(@"\", "").Trim();

                if (_repository.UserCanViewPage(_userName))
                {
                    _retVal = true;
                }
            *************************************************/

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return retVal;
        }
    }
}