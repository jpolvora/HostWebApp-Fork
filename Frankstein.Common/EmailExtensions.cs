using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Frankstein.Common
{
    public static class EmailExtensions
    {
        public static void SendEmail(MailAddress addressFrom, MailAddress addressTo, string subject, string htmlOrPlainText, bool plain, Action<MailMessage, Exception> onError)
        {
            using (var client = new SmtpClient())
            {
                var msg = new MailMessage(addressFrom, addressTo)

                {
                    Subject = subject,
                    IsBodyHtml = false,
                    BodyEncoding = Encoding.UTF8,
                    HeadersEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8,
                    Body = null
                };
                if (plain)
                {
                    var plainView = AlternateView.CreateAlternateViewFromString(htmlOrPlainText, msg.BodyEncoding, "text/plain");
                    plainView.TransferEncoding = TransferEncoding.SevenBit;
                    msg.AlternateViews.Add(plainView);
                }
                else
                {
                    msg.IsBodyHtml = true;
                    msg.Body = htmlOrPlainText;
                }

                try
                {
                    client.Send(msg);
                    onError(msg, null);
                }
                catch (Exception ex)
                {
                    onError(msg, ex);
                }
            }
        }
    }
}
