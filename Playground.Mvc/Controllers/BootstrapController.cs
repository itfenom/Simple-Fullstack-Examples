using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class BootstrapController : BaseController
    {
        public ActionResult Accordion()
        {
            return View();
        }

        public ActionResult Carousel()
        {
            return View();
        }

        public ActionResult TabControls()
        {
            return View();
        }

        public ActionResult Alerts(string value = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "SUCCESS":
                        Success($"<b>{"SUCCESS"}</b> This is a 'Success' alert.", true);
                        break;

                    case "DANGER":
                        Danger($"<b>{"DANGER"}</b> This is a 'Danger' alert.", true);
                        break;

                    case "INFO":
                        Information($"<b>{"INFO"}</b> This is a 'Info' alert.", true);
                        break;

                    case "WARNING":
                        Warning($"<b>{"WARNING"}</b> This is a 'Warning' alert.", true);
                        break;
                }
            }
            return View();
        }

        public ActionResult TypeAhead()
        {
            var model = GetPersons();
            return View(model);
        }

        public ActionResult PersonDetails(int id)
        {
            var detailModel = (from a in GetPersons()
                                where a.ID == id
                                select a).FirstOrDefault();

            return View(detailModel);
        }

        [HttpGet]
        public ActionResult PersonDelete(int id)
        {
            var model = (from a in GetPersons()
                          where a.ID == id
                          select a).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public ActionResult PersonDelete(int id, string name = null)
        {
            return RedirectToAction("TypeAhead");
        }

        public ActionResult GetPersonsJson()
        {
            var retVal = GetPersons();

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        private List<Person> GetPersons()
        {
            var retVal = new List<Person>
            {
                new Person
                {
                    ID = 1,
                    Name = "Billy Hall",
                    Email = "bh@test.com",
                    LikesMusic = true,
                    Skills = new List<string>() {"C#", "Web Services", "Windows Services"}
                },
                new Person
                {
                    ID = 2,
                    Name = "Billy Haynes",
                    Email = "bh@t.com",
                    LikesMusic = true,
                    Skills = new List<string>() {"Operations", "Management"}
                },
                new Person
                {
                    ID = 3,
                    Name = "Carlene Perry",
                    Email = "cp@test.com",
                    LikesMusic = false,
                    Skills = new List<string>() {"Retails", "Customer Service"}
                },
                new Person
                {
                    ID = 4,
                    Name = "Carlene Knowles",
                    Email = "ck@test.com",
                    LikesMusic = true,
                    Skills = new List<string>() {"Employee Management", "Account Management", "Employee Training"}
                },
                new Person
                {
                    ID = 5,
                    Name = "David Full",
                    Email = "df@test.com",
                    LikesMusic = true,
                    Skills = new List<string>() {"C#", "Web Pages", "MVC"}
                },
                new Person
                {
                    ID = 6,
                    Name = "David Seppi",
                    Email = "ds@test.com",
                    LikesMusic = true,
                    Skills = new List<string>() {"C#", "WCF", "Windows Services"}
                },
                new Person
                {
                    ID = 7,
                    Name = "Dave Moss",
                    Email = "dm@test.com",
                    LikesMusic = true,
                    Skills = new List<string>() {"Retail"}
                }
            };


            return retVal;
        }
    }
}