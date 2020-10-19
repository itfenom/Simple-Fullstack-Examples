using System;
using System.Globalization;
using System.Net.Mail;
using Playground.Core.MessageBoxWrappers;
using Playground.Core.Utilities;

namespace Playground.Core.Diagnostics
{
    public static class EmailManager
    {
        private static bool _mailHasFailedBefore;
        private static readonly string server = "smtprelay-central.corp.qorvo.com";

        public static void EmailToTeam(string message, string subject = null)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add("Kashif.Mubarak@Qorvo.com");
                mailMessage.From = new MailAddress("Application@Qorvo.com", "Application");

                if (string.IsNullOrEmpty(subject))
                {
                    mailMessage.Subject = "AUTOMATIC NOTIFICATION";
                }
                else
                {
                    mailMessage.Subject = subject;
                }

                mailMessage.Body = "Machine Name: " + Environment.MachineName
                    + "\nUser Name: " + Environment.UserName
                    + "\nTimestamp: " + DateTime.Now.ToString(CultureInfo.InvariantCulture)
                    + "\nVersion: " + System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version
                    + "\n\n" + message;

                /*
                Attachment logFileAttachment = new Attachment(copiedLogFile);
                mailMessage.Attachments.Add(logFileAttachment);
                */

                SmtpClient client = new SmtpClient(server);
                client.SendCompleted += (s, e) =>
                {
                    client.Dispose();
                    mailMessage.Dispose();
                };

                client.SendAsync(mailMessage, null);
            }
            catch (SmtpException e)
            {
                Logger.Error(e.Message, e);

                if (!_mailHasFailedBefore)
                {
                    MessageBoxWithTextBox.Show("Application is trying to email its log file but the email failed."
                        + "\n\n" + HelperTools.ExceptionToString(e),
                        "Smtp Exception");
                    _mailHasFailedBefore = true;
                }
            }
        }
    }
}
