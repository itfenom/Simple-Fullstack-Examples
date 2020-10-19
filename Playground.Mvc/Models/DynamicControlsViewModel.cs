using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Models
{
    public class DynamicControlsViewModel
    {
        public IList<EmployeeInfoAndSkills> EmployeeData { get; set; }
    }

    public class EmployeeInfoAndSkills
    {
        public string Name { get; set; }                        //TextBox for Name
        public string Gender { get; set; }                      //TextBox for Gender
        public string EnumSelection { get; set; }               //Enum Selection Value from the dropDown
        public int SkillCount { get; set; }                     //TextBox for SkillCount
        public string CurrentSkills { get; set; }               //TextArea for CurrentSkills
        public bool Active { get; set; }                        //CheckBox for Active
        public List<SelectListItem> GenderList { get; set; }    //DropDown for Gender i.e., Female/Male
        public SelectList AvailableSkills { get; set; }         //ListBox for Skills i.e., C#, VisualBasic, Java, ASP.NET, MVC, Bootstrap, CSS, HTML
    }

    public enum EnumChoices
    {
        EnumChoice1,
        EnumChoice2,
        EnumChoice3,
        EnumChoice4
    }
}