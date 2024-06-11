using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using EO.Pdf;

namespace VT.Common.Utils
{
    public class PdfUtil
    {
        public static byte[] ConvertHtmlToPdfStream(string html)
        {
            
            var result = HtmlToPdf.ConvertHtml(html, new MemoryStream());
            if (!result.PdfDocument.Attachments.Any()) return null;
            var attachment = result.PdfDocument.Attachments.FirstOrDefault();
            return attachment != null ? attachment.Data : null;
        }

        public static Attachment GetPdfAttachment(string htmlTemplate, string attachmentName)
        {
            if (string.IsNullOrEmpty(htmlTemplate)) return null;
            
            HtmlToPdf.Options.AutoFitX = HtmlToPdfAutoFitMode.None;
            HtmlToPdf.Options.AutoFitY = HtmlToPdfAutoFitMode.None;
            HtmlToPdf.Options.OutputArea = new RectangleF(0.0f, 0.0f, 8.5f, 11.0f);
             
            var memStream = new MemoryStream();
            HtmlToPdf.ConvertHtml(htmlTemplate, memStream);
            memStream.Position = 0;
            var ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
            var attach = new Attachment(memStream, ct);
            attach.ContentDisposition.FileName = attachmentName;
            return attach;
        }

        public static byte[] GetPdfMemoryStream(string htmlTemplate)
        {
            if (string.IsNullOrEmpty(htmlTemplate)) return null;

            HtmlToPdf.Options.AutoFitX = HtmlToPdfAutoFitMode.None;
            HtmlToPdf.Options.AutoFitY = HtmlToPdfAutoFitMode.None;
            HtmlToPdf.Options.OutputArea = new RectangleF(0.0f, 0.0f, 8.5f, 11.0f);

            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                HtmlToPdf.ConvertHtml(htmlTemplate, ms);
                //ms.Position = 0;
                int read;
                while ((read = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
