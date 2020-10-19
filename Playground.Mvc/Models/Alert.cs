namespace Playground.Mvc.Models
{
    public class Alert
    {
        public const string TempDataKey = "TempDataAlerts";

        public string AlertStyle { get; set; }
        public string Message { get; set; }
        public bool Dismissible { get; set; }
    }

    public enum AlertStyles
    {
        Success,
        Info,
        Warning,
        Danger
    }
}