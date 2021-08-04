using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper
{
    public class EmailHelper : IEmailHelper
    {
        private readonly ILogger<SaveAdminPeriodCommandHandler> _logger;
        private readonly AppSettings _setting;

        public EmailHelper(ILogger<SaveAdminPeriodCommandHandler> logger, 
                            AppSettings setting)
        {
            _setting = setting;
            _logger = logger;
        }

        public bool SendEmail(string emailto, string subject, string body)
        {
            try
            {
                //other logic
                using (var client = new SmtpClient(_setting.Smtp.Server, _setting.Smtp.Port))
                {
                    byte[] data = System.Convert.FromBase64String(_setting.Smtp.Password);
                    var passwordDecoded = System.Text.ASCIIEncoding.ASCII.GetString(data);

                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_setting.Smtp.User, passwordDecoded);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.TargetName = "STARTTLS/smtp.office365.com";

                    MailAddress from = new MailAddress(_setting.Smtp.User, "Application Admin", System.Text.Encoding.UTF8);
                    MailMessage message = new MailMessage();
                    message.From = from;

                    if (_setting.Smtp.IsTest)
                    {
                        string[] tests = _setting.Smtp.EmailTest.Split(";", StringSplitOptions.RemoveEmptyEntries);
                        foreach (string test in tests)
                        {
                            message.To.Add(test);
                        }

                        
                    }
                    else
                    {
                        message.To.Add(emailto);
                    }

                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    client.Send(message);    
                }



                return true;
            }
            catch (Exception ep)
            {
                _logger.LogError(ep.Message, ep);
                return false;
            }
        }

        public bool SendEmail(List<string> emailtos, string subject, string body)
        {
            try
            {
                //other logic//_setting.Smtp.Port
                using (var client = new SmtpClient(_setting.Smtp.Server, 587))
                {
                    byte[] data = System.Convert.FromBase64String(_setting.Smtp.Password);
                    var passwordDecoded = System.Text.ASCIIEncoding.ASCII.GetString(data);

                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_setting.Smtp.User, passwordDecoded);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.TargetName = "STARTTLS/smtp.office365.com";

                    MailAddress from = new MailAddress(_setting.Smtp.User, "Application Admin", System.Text.Encoding.UTF8);
                    MailMessage message = new MailMessage();
                    message.From = from;

                    if (_setting.Smtp.IsTest)
                    {
                        message.To.Add(_setting.Smtp.EmailTest);
                    }
                    else
                    {
                        foreach (var mail in emailtos)
                        {
                            message.To.Add(mail);
                        }
                    }
                    

                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    client.Send(message);
                }



                return true;
            }
            catch (Exception ep)
            {
                _logger.LogError(ep.Message, ep);
                return false;
            }
        }
    }
}
