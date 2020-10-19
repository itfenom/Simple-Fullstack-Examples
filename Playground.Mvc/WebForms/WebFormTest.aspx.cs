using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Playground.Mvc.WebForms
{
    public partial class WebFormTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var _postedModel = Session["PostedModel"] as List<EmployeeViewModel>;

                if (_postedModel != null)
                {
                    gridViewPerson.DataSource = _postedModel;
                    gridViewPerson.DataBind();
                }
            }
        }
    }
}