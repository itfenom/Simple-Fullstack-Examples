using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class BaseController : Controller
    {
        public void Success(string message, bool dismissible = false)
        {
            AddAlert(AlertStyles.Success.ToString(), message, dismissible);
        }

        public void Information(string message, bool dismissible = false)
        {
            AddAlert(AlertStyles.Info.ToString(), message, dismissible);
        }

        public void Warning(string message, bool dismissible = false)
        {
            AddAlert(AlertStyles.Warning.ToString(), message, dismissible);
        }

        public void Danger(string message, bool dismissible = false)
        {
            AddAlert(AlertStyles.Danger.ToString(), message, dismissible);
        }

        private void AddAlert(string alertStyle, string message, bool dismissible)
        {
            var alerts = TempData.ContainsKey(Alert.TempDataKey)
                ? (List<Alert>)TempData[Alert.TempDataKey]
                : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissible = dismissible
            });

            TempData[Alert.TempDataKey] = alerts;
        }
    }
}