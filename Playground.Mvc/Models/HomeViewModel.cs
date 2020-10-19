using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Playground.Mvc.Models
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "To email address is required")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Email Address:")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailTo { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Subject:")]
        public string EmailSubject { get; set; }

        [Required(ErrorMessage = "From email address is required")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Email Address:")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailFrom { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string EmailFromPassword { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Body:")]
        [DataType(DataType.MultilineText)]
        public string EmailBody { get; set; }

        [Display(Name = "Attachment")]
        public List<HttpPostedFileBase> File { get; set; }

        public string SelectedTemplate { get; set; }

        public List<EmailTemplate> EmailTemplates { get; set; }
    }

    public class EmailTemplate
    {
        // ReSharper disable once InconsistentNaming
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public bool? IsSelected { get; set; }
    }

    public class DepartmentViewModel
    {
        // ReSharper disable once InconsistentNaming
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public bool? IsSelectedDepartment { get; set; }
    }

    public class ContactViewModel
    {
        public string Name { get; set; }
        public ContactAddress Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Msg { get; set; }
    }

    public class ContactAddress
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class IslamicDateAndPrayerTimes
    {
        public string FajrTime { get; set; }
        public string DuhrTime { get; set; }
        public string AsrTime { get; set; }
        public string MaghribTime { get; set; }
        public string IshaTime { get; set; }
        public string HijriMonthInEnglish { get; set; }
        public string HijriMonthInArabic { get; set; }
        public string HijriDay { get; set; }
        public string HijriYear { get; set; }
        public string DayOfTheWeekInArabic { get; set; }

        public string TodayDate
        {
            get
            {
                return string.Format("{0} | {1}", DateTime.Now.DayOfWeek.ToString(), DateTime.Now.ToString("MMMM dd, yyyy"));
            }
        }
    }
}