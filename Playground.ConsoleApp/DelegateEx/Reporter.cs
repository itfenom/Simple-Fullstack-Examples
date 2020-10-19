using System;

namespace Playground.ConsoleApp.DelegateEx
{
    public class Reporter
    {
        public string GetDailyReport(int dayOfYear)
        {
            Console.WriteLine("Preparing daily report...");
            return "Report of day: " + dayOfYear;
        }

        public string GetWeeklyReport(int weekOfYear)
        {
            Console.WriteLine("Preparing weekly report...");
            return "Report of Week: " + weekOfYear;
        }

        public string GetMonthlyReport(int monthOfYear)
        {
            Console.WriteLine("Preparing monthly report...");
            return "Report of month: " + monthOfYear;
        }

        public string GetAnnualReport(int year)
        {
            Console.WriteLine("Preparing annual report...");
            return "Report of year: " + year;
        }

    }

    public enum ReportType
    {
        Daily,
        Weekly,
        Monthly,
        Annually
    }
}
