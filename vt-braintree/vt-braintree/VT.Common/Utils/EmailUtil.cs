using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using EO.Pdf;

namespace VT.Common.Utils
{
    public class EmailUtil
    {
        public static string Send(string from, string to, string subject, string body, string pdfHtml = null)
        {
            if (to == null) return string.Empty;

            try
            {
                // Send email.
                var message = new MailMessage
                {
                    Subject = subject,
                    From = new MailAddress(from),
                    Body = body,
                    IsBodyHtml = true,

                };

                message.To.Add(to);

                using (var memStream = new MemoryStream())
                {
                    if (!string.IsNullOrEmpty(pdfHtml))
                    {
                        var result = HtmlToPdf.ConvertHtml(pdfHtml, memStream);
                        memStream.Position = 0;
                        var ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                        var attach = new Attachment(memStream, ct);
                        attach.ContentDisposition.FileName = "verify-MMDDYYYY.pdf";
                        message.Attachments.Add(attach);
                    }

                    SmtpClient smtp = null;

#if !DEBUG
                smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("web2it0302@gmail.com", "@formless*"),
                    Timeout = 100000
                };

#else
                    smtp = new SmtpClient
                    {
                        Host = ApplicationSettings.Host,
                        Port = ApplicationSettings.Port,
                        EnableSsl = ApplicationSettings.IsSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(ApplicationSettings.Username, ApplicationSettings.Password),
                        Timeout = 100000
                    };

#endif
                    smtp.Send(message);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return string.Empty;
        }

        public static string Send(string from, string to, string subject, string body, List<Attachment> attachments = null)
        {
            if (to == null) return string.Empty;


#if DEBUG
            //to = "karanjamwal@gmail.com";
#endif  

            try
            {
                // Send email.
                var message = new MailMessage
                {
                    Subject = subject,
                    From = new MailAddress(from),
                    Body = body,
                    IsBodyHtml = true,

                };

                message.To.Add(to);

                if (attachments != null)
                {
                   foreach (var attachment in attachments)
                        message.Attachments.Add(attachment);
                }
                    

                var smtp = new SmtpClient
                {
                    Host = ApplicationSettings.Host,
                    Port = ApplicationSettings.Port,
                    EnableSsl = ApplicationSettings.IsSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ApplicationSettings.Username, ApplicationSettings.Password),
                    Timeout = 100000
                };
                smtp.Send(message);

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        if (attachment != null && attachment.ContentStream != null)
                        {
                            try
                            {
                                attachment.ContentStream.Close();
                            }
                            catch (Exception exception)
                            { 
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return string.Empty;
        }
    }
}
