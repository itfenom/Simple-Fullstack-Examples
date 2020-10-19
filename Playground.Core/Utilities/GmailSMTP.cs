using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net.Mail;

namespace Playground.Core.Utilities
{
    // ReSharper disable once IdentifierTypo
    public class GmailSmtp
    {
        public static readonly string ClassName = MethodBase.GetCurrentMethod().DeclaringType?.ToString();

        private string _smtpServerHost;
        private int _smtpPort;
        private string _smtpFrom;
        private string _smtpTo;
        private string _smtpUserId;
        private string _smtpUserPassword;

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

        public string SmtpUserID
        {
            get => _smtpUserId;
            set => _smtpUserId = value;
        }

        public string SmtpUserPassword
        {
            get => _smtpUserPassword;
            set => _smtpUserPassword = value;
        }

        public GmailSmtp()
        {

        }

        static GmailSmtp()
        {

        }

        public GmailSmtp(string serverHost, int port, string from, string to, string userId, string userPassword)
        {
            _smtpServerHost = serverHost;
            _smtpPort = port;
            _smtpFrom = from;
            _smtpTo = to;
            _smtpUserId = userId;
            _smtpUserPassword = userPassword;
        }

        public bool Send(string body, string subject, List<string> attachment)
        {
            return SendEmail(_smtpServerHost, _smtpPort, _smtpUserId, _smtpUserPassword, _smtpFrom, _smtpTo, body, subject, attachment);
        }

        public static bool SendEmail(string server, int port, string userId, string userPassword, string from, string to, string body, string subject, List<string> attachments)
        {
            string prePostBodyMsg;
            SmtpClient smtp = new SmtpClient(server, port)
            {
                Credentials = new System.Net.NetworkCredential(userId, userPassword), EnableSsl = true
            };
            MailMessage mm = new MailMessage();

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
