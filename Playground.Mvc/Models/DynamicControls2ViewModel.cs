using System;
using System.Collections.Generic;

namespace Playground.Mvc.Models
{
    public class DynamicControls2ViewModel
    {
        public IList<EmployeeInfo> EmployeeInfo { get; set; }
    }

    public class EmployeeInfo
    {
        public string EmployeeName { get; set; }
        public string Location { get; set; }
        public Gender Sex { get; set; }
        public int DisplayOrder { get; set; }
        public string Skills { get; set; }
        public string EditDetailsFlag { get; set; }

        public Boolean IsCurrentEmployee { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Unknown
    }
}