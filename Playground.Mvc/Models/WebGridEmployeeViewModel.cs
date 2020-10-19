using System.Collections.Generic;

namespace Playground.Mvc.Models
{
    public class WebGridEmployeeViewModel
    {
        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        public int Pages { get; set; }
        public int Rows { get; set; }
    }
}