using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class DynamicControlsController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new DynamicControlsViewModel()
            {
                EmployeeData = GetModel()
            };

            var itemCount = GetModel().Count();
            ViewBag.itemCount = itemCount;
            ViewData["Choices"] = Enum.GetNames(typeof(EnumChoices));
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DynamicControlsViewModel postedModel)
        {
            ViewData["Choices"] = Enum.GetNames(typeof(EnumChoices));
            if (ModelState.IsValid)
            {
                return View("PostedData", postedModel);
            }
            else
            {
                ModelState.AddModelError("", "Invalid Data. Try gain.");
            }
            return View(postedModel);
        }

        public ActionResult LoadSkillsIntoPartialView(string skillCount, string employeeName)
        {
            var partialViewModel = new EmployeeInfoAndSkills();

            partialViewModel.AvailableSkills = GetSkills();

            // ReSharper disable once UseMethodAny.2
            if (partialViewModel.AvailableSkills == null || partialViewModel.AvailableSkills.Count() == 0)
            {
                //if there's were not skills, then display an empty dialog box!!!
                return Content("<div><p style='color:red;'><strong>No Data to display!<strong></p></div>");
                //return HttpNotFound();
            }

            return PartialView("_Dialog", partialViewModel);
        }

        private List<EmployeeInfoAndSkills> GetModel()
        {
            var model = new List<EmployeeInfoAndSkills>
            {
                new EmployeeInfoAndSkills
                {
                    Name = "Billy Hall",
                    Gender = "Male",
                    EnumSelection = "EnumChoice1",
                    Active = true,
                    SkillCount = 4,
                    CurrentSkills = "C#, VB, T-SQL, PL/SQL",
                    GenderList = GetGenderList("Male")
                },
                new EmployeeInfoAndSkills
                {
                    Name = "David Seppi",
                    Gender = "Male",
                    EnumSelection = "EnumChoice2",
                    Active = false,
                    SkillCount = 3,
                    CurrentSkills = "C#, T-SQL, PL/SQL",
                    GenderList = GetGenderList("Male")
                },
                new EmployeeInfoAndSkills
                {
                    Name = "David Haynes",
                    Gender = "Male",
                    EnumSelection = "EnumChoice3",
                    Active = true,
                    SkillCount = 2,
                    CurrentSkills = "C#, PL/SQL",
                    GenderList = GetGenderList("Male")
                },
                new EmployeeInfoAndSkills
                {
                    Name = "Mary Mix",
                    Gender = "Female",
                    EnumSelection = "EnumChoice3",
                    Active = true,
                    SkillCount = 2,
                    CurrentSkills = "C#, PL/SQL",
                    GenderList = GetGenderList("Female")
                },
                new EmployeeInfoAndSkills
                {
                    Name = "Mary Jones",
                    Gender = "Female",
                    EnumSelection = "EnumChoice4",
                    Active = false,
                    SkillCount = 1,
                    CurrentSkills = "VB",
                    GenderList = GetGenderList("Female")
                }
            };
            return model;
        }

        private List<SelectListItem> GetGenderList(string selectedGender)
        {
            var retVal = new List<SelectListItem>();

            if (selectedGender.ToUpper() == "MALE")
            {
                retVal.Add(new SelectListItem { Text = "Male", Value = "Male", Selected = true });
                retVal.Add(new SelectListItem { Text = "Female", Value = "Female", Selected = false });
            }
            else if (selectedGender.ToUpper() == "FEMALE")
            {
                retVal.Add(new SelectListItem { Text = "Male", Value = "Male", Selected = false });
                retVal.Add(new SelectListItem { Text = "Female", Value = "Female", Selected = true });
            }

            return retVal;
        }

        private SelectList GetSkills()
        {
            var skills = new List<string>
            {
                "C#",
                "Visual Basic",
                "Java",
                "ASP.NET",
                "MVC",
                "PL/SQL",
                "T-SQL"
            };


            return new SelectList(skills);
        }
    }
}