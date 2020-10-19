using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var islamicDateAndPrayTimeModel = GetPrayerTimesAndDateModel();

            ViewBag.IslamicDate = islamicDateAndPrayTimeModel.TodayDate + " | " + islamicDateAndPrayTimeModel.HijriDay + " " + islamicDateAndPrayTimeModel.HijriMonthInEnglish + ", " + islamicDateAndPrayTimeModel.HijriYear + " Hijri";

            return View();
        }

        private string Convert24HrsTo12Hrs(string timeIn24Hrs)
        {

            var dateTime = DateTime.ParseExact(timeIn24Hrs, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);

            var retVal = Convert.ToDateTime(dateTime).ToString("hh:mm tt");

            return retVal.TrimStart('0');
        }

        private IslamicDateAndPrayerTimes GetPrayerTimesAndDateModel()
        {
            var retVal = new IslamicDateAndPrayerTimes();

            var prayerTimes = new PrayTime();
            var longitude = 32.91; // 96.48; //25;
            var lattitude = -96.64; // 32.47; // 55;

            var currentDateTime = DateTime.Now;
            var year = currentDateTime.Year;
            var month = currentDateTime.Month;
            var  day = currentDateTime.Day;
            var timeZone = TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime(year, month, day)).Hours;

            prayerTimes.SetCalcMethod(3);
            prayerTimes.SetAsrMethod(1);
            var prayers = prayerTimes.GetDatePrayerTimes(year, month, day, longitude, lattitude, timeZone);

            retVal.FajrTime = Convert24HrsTo12Hrs(prayers[0]);
            retVal.DuhrTime = Convert24HrsTo12Hrs(prayers[2]);
            retVal.AsrTime = Convert24HrsTo12Hrs(prayers[3]);
            retVal.MaghribTime = Convert24HrsTo12Hrs(prayers[4]);
            retVal.IshaTime = Convert24HrsTo12Hrs(prayers[6]);

            //Get Hijri Date
            var dt = DateTime.Today.Date.AddDays(0);
            // ReSharper disable once IdentifierTypo
            System.Globalization.DateTimeFormatInfo hijriDtfi; //Hijri Date Format Info
            hijriDtfi = new System.Globalization.CultureInfo("ar-SA", false).DateTimeFormat;
            hijriDtfi.Calendar = new System.Globalization.UmAlQuraCalendar();

            retVal.HijriYear = dt.Date.ToString("yyyy", hijriDtfi);            // Gets the Year i.e., 1436 etc.
            retVal.HijriDay = dt.Date.ToString("dd", hijriDtfi);               // Gets the date i.e., 25th of the month etc.
            retVal.DayOfTheWeekInArabic = dt.Date.ToString("dddd", hijriDtfi); // Gets the day of the week in Arabic i.e., Khamees (Thursday) etc.
            retVal.HijriMonthInArabic = dt.Date.ToString("MMMM", hijriDtfi);   // Gets the Month in Arabic i.e., Rajab etc.

            for (int i = 0; i <= hijriDtfi.MonthGenitiveNames.Count(); i++)
            {
                if (retVal.HijriMonthInArabic == hijriDtfi.MonthGenitiveNames[i])
                {
                    retVal.HijriMonthInEnglish = GetHijriMonthInEnglish(i + 1); // Gets the Month in English i.e., Rajab etc.
                    break;
                }
            }

            return retVal;
        }

        // ReSharper disable once IdentifierTypo
        private string GetHijriMonthInEnglish(int monthNumber)
        {
            string retVal = string.Empty;

            switch (monthNumber)
            {
                case 1:
                    retVal = "Muharram";
                    break;

                case 2:
                    retVal = "Safar";
                    break;

                case 3:
                    retVal = "Rabī‘ al-Awwal";
                    break;

                case 4:
                    retVal = "Rabī‘ ath-Thānī";
                    break;

                case 5:
                    retVal = "Jumādá al-Ūlá";
                    break;

                case 6:
                    retVal = "Jumādá ath-Thāniyah";
                    break;

                case 7:
                    retVal = "Rajab";
                    break;

                case 8:
                    retVal = "Sha'ban";
                    break;

                case 9:
                    retVal = "Ramadan";
                    break;

                case 10:
                    retVal = "Shawwal";
                    break;

                case 11:
                    retVal = "Dhu al-Qa'dah";
                    break;

                case 12:
                    retVal = "Dhu al-Hijjah";
                    break;
            }

            return retVal;
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "My MVC Playground using Entity Framework 5.0.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ContactViewModel model = new ContactViewModel()
            {
                Name = "Kashif Mubarak",
                Address = new ContactAddress()
                {
                    StreetAddress = "123 Main St.",
                    City = "Somewhere",
                    State = "XX",
                    Zip = "#####"
                },
                Email = "mubarak.kashif@gmail.com",
                Phone = "(###) ###-####"
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel model)
        {
            if (string.IsNullOrEmpty(model.Msg))
            {
                ModelState.AddModelError("Msg", "Message is required!");
                return View(model);
            }

            ViewBag.Message = "Thank you for the message!";
            return PartialView("_ContactThanks");
        }

        public ActionResult ViewDataViewBagTempData()
        {
            List<string> latestTechnology = new List<string>
            {
                "C#",
                "Visual Basic",
                "ASP.NET",
                "MVC",
                "Entity Framework",
                "SQL Server",
                "Oracle"
            };

            ViewBag.LatestTechnologies = latestTechnology;
            ViewData["LatestTechnologies"] = latestTechnology;

            return View();
        }

        public ActionResult HtmlHelper()
        {
            var selectListItems = new List<SelectListItem>();
            var model = GetDepartmentList();

            foreach (DepartmentViewModel department in model)
            {
                var selectListItem = new SelectListItem()
                {
                    Text = department.DepartmentName,
                    Value = department.DepartmentID.ToString(),
                    // ReSharper disable once SimplifyConditionalTernaryExpression
                    Selected = department.IsSelectedDepartment.HasValue ? department.IsSelectedDepartment.Value : false
                };
                selectListItems.Add(selectListItem);
            }
            ViewBag.Departments = selectListItems;
            return View();
        }

        private List<DepartmentViewModel> GetDepartmentList()
        {
            var retVal = new List<DepartmentViewModel>
            {
                new DepartmentViewModel {DepartmentID = 1, DepartmentName = "IT", IsSelectedDepartment = false},
                new DepartmentViewModel {DepartmentID = 2, DepartmentName = "HR", IsSelectedDepartment = true},
                new DepartmentViewModel {DepartmentID = 3, DepartmentName = "Payroll", IsSelectedDepartment = false}
            };
            return retVal;
        }

        [HttpGet]
        public ActionResult SendEmail()
        {
            var model = new EmailViewModel {EmailTemplates = GetEmailTemplateValues()};
            return View(model);
        }

        private List<EmailTemplate> GetEmailTemplateValues()
        {
            var retVal = new List<EmailTemplate>
            {
                new EmailTemplate {TemplateID = 1001, TemplateName = "Ezaan-Rayyan Template", IsSelected = false},
                new EmailTemplate {TemplateID = 1002, TemplateName = "Simple Template", IsSelected = false},
                new EmailTemplate {TemplateID = 1003, TemplateName = "None", IsSelected = true}
            };


            return retVal;
        }

        [HttpPost]
        public ActionResult SendEmail(EmailViewModel requestModel)
        {
            requestModel.EmailTemplates = GetEmailTemplateValues();

            if (!ModelState.IsValid)
            {
                return View(requestModel);
            }
            else
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    string msgBody;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(requestModel.EmailFrom, requestModel.EmailFromPassword);

                    var mail = new MailMessage {From = new MailAddress(requestModel.EmailFrom)};
                    mail.To.Add(requestModel.EmailTo);
                    mail.Subject = requestModel.EmailSubject;

                    if (requestModel.SelectedTemplate != "1003")
                    {
                        string[] parts;
                        requestModel.EmailBody = requestModel.EmailBody.Replace(Environment.NewLine, "<br/>");
                        mail.IsBodyHtml = true;
                        using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(SelectMailTemplate(requestModel.SelectedTemplate))))
                        {
                            msgBody = reader.ReadToEnd();
                        }

                        if (requestModel.SelectedTemplate == "1001")
                        {
                            parts = requestModel.EmailTo.Split('.');
                            msgBody = msgBody.Replace("{Name}", parts[0]);
                            msgBody = msgBody.Replace("{messageBody}", requestModel.EmailBody);
                        }
                        else if (requestModel.SelectedTemplate == "1002")
                        {
                            parts = requestModel.EmailTo.Split('@');
                            msgBody = msgBody.Replace("{Name}", parts[0]);
                            msgBody = msgBody.Replace("{messageBody}", requestModel.EmailBody);
                        }

                        mail.Body = msgBody;
                    }
                    else if (requestModel.SelectedTemplate == "1003")
                    {
                        mail.Body = requestModel.EmailBody;
                    }

                    if (requestModel.File[0] != null)
                    {
                        foreach (HttpPostedFileBase item in requestModel.File)
                        {
                            var attachment = new Attachment(item.InputStream, item.FileName);
                            mail.Attachments.Add(attachment);
                        }
                    }

                    client.Send(mail);
                    Success($"<b>{"SUCCESS"}</b> Message sent succcessfully!.", true);
                }
            }
            return RedirectToAction("SendEmail");
        }

        private string SelectMailTemplate(string selectedTemplateVal)
        {
            string retVal = string.Empty;

            if (selectedTemplateVal == "1001")
            {
                retVal = @"~/EmailTemplates/Ezaan_Rayyan_Mail_Template.html";
            }
            else if (selectedTemplateVal == "1002")
            {
                retVal = @"~/EmailTemplates/Simple_Mail_Template.html";
            }

            return retVal;
        }
    }
}