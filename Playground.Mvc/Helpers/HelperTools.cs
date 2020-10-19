using System;
using System.Web.Mvc;

namespace Playground.Mvc.Helpers
{
    public static class HelperTools
    {
        //Creates Hyperlink!
        public static MvcHtmlString HyperLink(this HtmlHelper html, string url, string linkText)
        {
            return MvcHtmlString.Create($"<a href='{url}'>{linkText}</a>");
        }
    }
}