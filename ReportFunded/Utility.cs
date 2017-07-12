using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using log4net;

public static class Utility
{

    private static readonly ILog log = LogManager.GetLogger(typeof(Utility));

    public static void SendEmail(List<String> to, List<String> cc, List<String> bcc, String subject, String bodyText)
    {
        log.Info("Generating email...");

        MailMessage message = new MailMessage();

        message.IsBodyHtml = true;
        foreach(String address in to){
            message.To.Add(new MailAddress(address));
        }
        foreach (String address in cc)
        {
            message.CC.Add(new MailAddress(address));
        }
        foreach (String address in bcc)
        {
            message.Bcc.Add(new MailAddress(address));
        }
        message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["from"].ToString(), "Reporting Server");
        message.Subject = subject;
        message.Body = bodyText;
        

        SmtpClient smtp = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["smtp_server"].ToString());
        smtp.Send(message);
        

        log.Info("Email sent!");
    }
    public static Session ConnectToServer()
    {
        try
        {
            log.Info("connecting to server...");

            Session s = new Session();
            s.Start(System.Configuration.ConfigurationManager.AppSettings["Eserver_address"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Eserver_login"].ToString(),
            System.Configuration.ConfigurationManager.AppSettings["Eserver_pw"].ToString());
            log.Info("connected.");
            return s;
        }
        catch (Exception e)
        {
            log.Error(e);
            log.Info("shutting down...");
            Environment.Exit(10);
            return null;
        }
        
    }
    public static String toShortDate(Object datetime)
    {
        DateTime temp = Convert.ToDateTime(datetime);
        if (temp == DateTime.MinValue)
        {
            return " ";
        }
        else
        {
            return temp.ToShortDateString();
        }
    }
    public static String toPercent(Object percent)
    {
        Double temp = Convert.ToDouble(percent);

        //String temp = Convert.ToDouble(percent).ToString("F3");
        
        if (temp<.000001)
        {
            return " ";
        }
        else
        {
            return temp.ToString("F3");
        }
    }


}
