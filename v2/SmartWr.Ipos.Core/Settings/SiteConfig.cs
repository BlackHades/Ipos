using System;
using System.Configuration;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using SmartWr.Ipos.Core.Context;

namespace SmartWr.Ipos.Core.Settings
{
    public static class IposConfig
    {
        public static string AppName
        {
            get
            {
                return ConfigurationManager.AppSettings["IposAppName"]
                    ?? Assembly.GetExecutingAssembly().FullName;
            }
        }

        public static string AutomatedFromEmail
        {
            get
            {
                var mailAcct = ConfigurationManager.AppSettings["IposMailer"];

                if (string.IsNullOrEmpty(mailAcct))
                    throw new Exception("IposMailer setting missing from configuration file.");
                return mailAcct;
            }
        }

        public static SmtpClient GetSmtpClient()
        {
            SmtpClient client = new SmtpClient();
            if (client.Host == null && client.Credentials == null && client.DeliveryMethod == SmtpDeliveryMethod.Network)
            {
                client.Host = SmtpServer;
                client.Port = SmtpPort;
                client.EnableSsl = SmtpEnableSSl;
                client.UseDefaultCredentials = SmtpUseDefaultCredentials;
                if (!SmtpUseDefaultCredentials)
                    client.Credentials = new NetworkCredential(SmtpLogin, SmtpPassword);
            }
            return client;
        }

        public static void ToggleDbInitializer()
        {
            if (RemoveDbInitializer)
                Database.SetInitializer<IPosDbContext>(null);
        }

        public static string SmtpServer
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpServer"];
            }
        }

        private static bool RemoveDbInitializer
        {
            get
            {
                var val = false;
                bool.TryParse(ConfigurationManager.AppSettings["RemoveDbInitializer"], out val);
                return val;
            }
        }

        public static int SmtpPort
        {
            get
            {
                var val = 0;
                int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out val);
                return val;
            }
        }

        public static bool SmtpEnableSSl
        {
            get
            {
                var val = false;
                bool.TryParse(ConfigurationManager.AppSettings["EnableSSl"], out val);
                return val;
            }
        }

        public static bool SmtpUseDefaultCredentials
        {
            get
            {
                var val = false;
                bool.TryParse(ConfigurationManager.AppSettings["SmtpUseDefaultCredentials"], out val);
                return val;
            }
        }

        public static string SmtpLogin
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpLogin"];
            }
        }

        public static string SmtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpPassword"];
            }
        }

        public static string SmsHost
        {
            get
            {
                return ConfigurationManager.AppSettings["SmsHost"];
            }
        }

        public static string SmsLogin
        {
            get
            {
                return ConfigurationManager.AppSettings["SmsLogin"];
            }
        }

        public static string SmsPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SmsPassword"];
            }
        }
        public static bool UseMembership
        {
            get
            {
                var val = false;
                bool.TryParse(ConfigurationManager.AppSettings["UseMembership"], out val);
                return val;
            }
        }

    }
}