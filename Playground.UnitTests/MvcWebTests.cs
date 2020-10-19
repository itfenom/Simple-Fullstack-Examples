using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Playground.UnitTests
{
    [TestClass]
    public class MvcWebTests
    {
        [TestMethod]
        public void JsonResultTest()
        {
            //arrange
            var jsonController = new JsonController();
            var data = jsonController.GetJsonData();
            var itemFound = false;

            //act
            foreach (var item in data.Data as dynamic)
            {
                var itemText = item.Text.ToString();
                if (itemText == "World")
                {
                    itemFound = true;
                    break;
                }
            }

            //assert
            Assert.IsTrue(itemFound);
        }

    }

    public class JsonController : Controller
    {
        public JsonResult GetJsonData()
        {
            var selectList = new List<SelectListItem>
            {
                new SelectListItem {Value = "Hello", Text = "Hello"},
                new SelectListItem {Value = "World", Text = "World"},
                new SelectListItem {Value = "This", Text = "This"},
                new SelectListItem {Value = "is", Text = "is"},
                new SelectListItem {Value = "a", Text = "a"},
                new SelectListItem {Value = "Test!", Text = "Test!"}
            };

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
    }
}
