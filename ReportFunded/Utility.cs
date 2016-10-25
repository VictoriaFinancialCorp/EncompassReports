using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Net.Mail;

public static class Utility
{
    public static void SendEmail(List<String> to, List<String> cc, String subject, String bodyText)
    {
        Console.Out.WriteLine("Generating email...");

        MailMessage message = new MailMessage();

        message.IsBodyHtml = true;
        foreach(String address in to){
            message.To.Add(new MailAddress(address));
        }
        foreach (String address in cc)
        {
            message.CC.Add(new MailAddress(address));
        }
        message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Eserver_login"].ToString(), "Local Server");
        message.Subject = subject;
        message.Body = bodyText;
        

        SmtpClient smtp = new SmtpClient("localhost");
        smtp.Send(message);
        

        Console.Out.WriteLine("Email sent!");
    }
    public static Session ConnectToServer()
    {
        Console.Out.WriteLine("connecting to server...");
        Session s = new Session();
        s.Start(System.Configuration.ConfigurationManager.AppSettings["Eserver_address"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Eserver_login"].ToString(),
        System.Configuration.ConfigurationManager.AppSettings["Eserver_pw"].ToString());
        Console.Out.WriteLine("connected.");
        return s;
    }
}
