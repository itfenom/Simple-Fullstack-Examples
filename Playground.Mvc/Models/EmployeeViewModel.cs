using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Playground.Mvc.Models
{
    public class EmployeeViewModel
    {
        // ReSharper disable once InconsistentNaming
        public int EmpID { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Employee Name cannot be longer than 20 characters")]
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email!")]
        [Display(Name = "Employee Email")]
        public string EmpEmail { get; set; }

        [Required]
        [Display(Name = "Employee Phone")]
        public string EmpPhone { get; set; }

        [Required]
        [Display(Name = "Employee Salary")]
        [Range(3000, 10000000, ErrorMessage = "Salary must be between 3000 and 10000000")]
        public int? EmpSalary { get; set; }

        [Required]
        [Display(Name = "Employee Gender")]
        [StringLength(1, ErrorMessage = "Employee Gender can only be 'M' or 'F'.")]
        public string EmpGender { get; set; }

        [Display(Name = "Hire date")]
        public DateTime? EmpHireDate { get; set; }

        [Display(Name = "Active?")]
        public bool? EmpIsActive { get; set; }

        [Display(Name = "Employee Photo")]
        public HttpPostedFileBase File { get; set; }

        public string Salary
        {
            get
            {
                if (EmpSalary == null)
                {
                    return "N/A";
                }

                return Convert.ToString(EmpSalary);
            }
        }

        public string HireDate
        {
            get
            {
                if (EmpHireDate == DateTime.MinValue)
                {
                    return "N/A";
                }

                return Convert.ToDateTime(EmpHireDate).ToString("MM/dd/yyyy");
            }
        }

        public string IsActive
        {
            get
            {
                string retVal = string.Empty;
                if (EmpIsActive == null)
                {
                    retVal = "No";
                }
                else if ((EmpIsActive != null) && (EmpIsActive == true))
                {
                    retVal = "Yes";
                }
                else if ((EmpIsActive != null) && (EmpIsActive == false))
                {
                    retVal = "No";
                }
                return retVal;
            }
        }

        public string EmpStatus { get; set; }

        public byte[] EmpPhoto { get; set; }
        public int? EmpPhotoId { get; set; }
    }
}