using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;

namespace Playground.Core.Utilities
{
    public class Smtp
    {
        public static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType?.ToString();

        private string _smtpServerHost;
        private int _smtpPort;
        private string _smtpFrom;
        private string _smtpTo;
        private string _smtpUserId;
        private string _smtpPassword;

        public string SmtpServerHost
        {
            get => _smtpServerHost;
            set => _smtpServerHost = value;
        }

        public int SmtpPort
        {
            get => _smtpPort;
            set => _smtpPort = value;
        }

        public string SmtpFrom
        {
            get => _smtpFrom;
            set => _smtpFrom = value;
        }

        public string SmtpTo
        {
            get => _smtpTo;
            set => _smtpTo = value;
        }

        public string SmtpUserId
        {
            get => _smtpUserId;
            set => _smtpUserId = value;
        }

        public string SmtpPassword
        {
            get => _smtpPassword;
            set => _smtpPassword = value;
        }

        public Smtp()
        {

        }

        static Smtp()
        {

        }

        public Smtp(string serverHost, int port, string from, string to, string id, string password)
        {
            _smtpServerHost = serverHost;
            _smtpPort = port;
            _smtpFrom = from;
            _smtpTo = to;
            _smtpUserId = id;
            _smtpPassword = password;
        }

        public bool Send(string body, string subject, List<string> attachment)
        {
            return SendEmail(_smtpServerHost, _smtpPort, _smtpFrom, _smtpTo, _smtpUserId, _smtpPassword, body, subject, attachment);
        }

        public static bool SendEmail(string server, int port, string from, string to, string id, string password, string body, string subject, List<string> attachments)
        {
            string prePostBodyMsg;
            var smtp = new SmtpClient(server, port);
            var mm = new MailMessage();

            prePostBodyMsg = "\r\r *** Do not reply to this message. *** \r\r";

            if (body == null)
            {
                body = prePostBodyMsg + prePostBodyMsg;
            }

            // ReSharper disable once ReplaceWithStringIsNullOrEmpty
            if (server == null || server.Length == 0 || from.Length == 0 || to.Length == 0)
                return false;
            if (subject.Length == 0)
                return false;

            try
            {
                smtp.Credentials = new System.Net.NetworkCredential(id, password);
                smtp.EnableSsl = true;

                mm.From = new MailAddress(from);
                mm.To.Add(to);
                mm.Subject = subject;
                mm.Body = prePostBodyMsg + body + prePostBodyMsg;

                if (attachments == null || attachments.Count == 0)
                {
                    smtp.Send(mm);
                    return true;
                }
                else if (attachments.Count > 0)
                {
                    foreach (string attachment in attachments)
                    {
                        var attach = new Attachment(attachment);
                        mm.Attachments.Add(attach);
                    }
                    smtp.Send(mm);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Failed to send email '" + subject + "' : " + ex));
            }
            finally
            {
                mm.Dispose();
                GC.Collect();
            }
            return true;
        }
    }
}
