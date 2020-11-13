using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CYQK.Test.Mail
{
    public class SendMail
    {
        public bool Send(string to1, string from1, string subject, string body, string userName, string password, string smtpHost, string sendcontect, string fmailpath)
        {
            try
            {
                MailAddress from = new MailAddress(from1);
                // MailAddress to = new MailAddress(to1);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.From = from;
                String[] list = to1.Split(';');
                foreach (var item in list)
                {
                    message.To.Add(item);
                }
                message.Subject = subject;//设置邮件主题
                message.IsBodyHtml = true;//设置邮件正文为html格式
                string fcontenct = body;
                string fqm = "信息部";
                string falltext = "";

                while (fcontenct.Length > 0)
                {
                    if (fcontenct.IndexOf("\r\n") >= 0)
                    {
                        falltext = falltext + "<font size=3   style= font-weight:bold >" + fcontenct.Substring(0, fcontenct.IndexOf("\r\n")) + "</font><br>";

                        fcontenct = fcontenct.Substring(fcontenct.IndexOf("\r\n") + 2, fcontenct.Length - fcontenct.IndexOf("\r\n") - 2);

                    }
                    else
                    {
                        falltext = falltext + "<font size=3   style= font-weight:bold >" + fcontenct + "</font><br>";

                        fcontenct = "";
                    }
                }
                falltext = falltext + "<br>";

                falltext = falltext + "<br>";

                falltext = falltext + "<font size=2  color=#045285   >----------------------------------------- </font><br>";
                while (fqm.Length > 0)
                {

                    if (fqm.IndexOf("\r\n") >= 0)
                    {
                        falltext = falltext + "<font size=2  color=#045285   >" + fqm.Substring(0, fqm.IndexOf("\r\n")) + "</font><br>";
                        fqm = fqm.Substring(fqm.IndexOf("\r\n") + 2, fqm.Length - fqm.IndexOf("\r\n") - 2);

                    }
                    else
                    {
                        falltext = falltext + "<font size=2  color=#045285   >" + fqm + "</font><br>";
                        fqm = "";
                    }
                }

                message.Body = falltext;//设置邮件内容
                //  message.Body = "<br><font size=2 color=#045285  > " + tqm.Text + "</font>";
                //      message. = textContent.Text ; 
                if (fmailpath != "")
                {
                    message.Attachments.Add(new Attachment(fmailpath, System.Net.Mime.MediaTypeNames.Application.Rtf));
                }
                SmtpClient client = new SmtpClient(smtpHost);
                //设置发送邮件身份验证方式

                //注意如果发件人地址是abc@def.com，则用户名是abc而不是abc@def.com
                client.Credentials = new NetworkCredential(userName, password);
                client.Send(message);
                return true;
            }
            catch (Exception ee)
            {
                return false;
            }

        }
    }
}
