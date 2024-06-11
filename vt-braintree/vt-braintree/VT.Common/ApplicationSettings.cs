using System;
using System.Configuration;

namespace VT.Common
{
    public static class ApplicationSettings
    {

        public static string AdminPassword
        {
            get { return ConfigurationManager.AppSettings["AdminPassword"]; }
        }

        public static bool EnableUserImport
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableUserImport"]); }
        }

        public static bool EnableCustomerImport
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCustomerImport"]); }
        }
        public static Double ServiceFeePercentage
        {
            get { return Convert.ToDouble(ConfigurationManager.AppSettings["ServiceFeePercentage"]); }
        }
        public static string FromEmail
        {
            get { return ConfigurationManager.AppSettings["FromEmail"]; }
        }

        public static string MerchantId
        {
            get { return ConfigurationManager.AppSettings["MerchantId"]; }
        }

        public static string PublicKey
        {
            get { return ConfigurationManager.AppSettings["PublicKey"]; }
        }

        public static string PrivateKey
        {
            get { return ConfigurationManager.AppSettings["PrivateKey"]; }
        }

        public static string MerchantAccountId
        {
            get { return ConfigurationManager.AppSettings["MerchantAccountId"]; }
        }

        public static string SecureBaseUrl
        {
            get { return ConfigurationManager.AppSettings["SecureBaseUrl"]; }
        }

        public static string AwsS3Url
        {
            get { return ConfigurationManager.AppSettings["AwsS3Url"]; }
        }

        public static string Host
        {
            get { return ConfigurationManager.AppSettings["Host"]; }
        }

        public static string Username
        {
            get { return ConfigurationManager.AppSettings["Username"]; }
        }

        public static string Password
        {
            get { return ConfigurationManager.AppSettings["Password"]; }
        }

        public static int Port
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["Port"]); }
        }

        public static bool IsSsl
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsSsl"]); }
        }

        public static int PhotoQuality
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["PhotoQuality"]); }
        }

        public static int PhotoWidth
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["PhotoWidth"]); }
        }

        public static int PhotoHeight
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["PhotoHeight"]); }
        }

        public static bool PhotoCrop
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["PhotoCrop"]); }
        }

        public static bool IsGatewayLive
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsGatewayLive"]); }
        }
         
        public static string Descriptor
        {
            get { return ConfigurationManager.AppSettings["Descriptor"]; }
        }

        public static string SuperAdminDescriptorName
        {
            get { return ConfigurationManager.AppSettings["SuperAdminDescriptorName"]; }
        }

        public static string SuperAdminDescriptorTelephone
        {
            get { return ConfigurationManager.AppSettings["SuperAdminDescriptorTelephone"]; }
        }

        public static string SuperAdminDescriptorUrl
        {
            get { return ConfigurationManager.AppSettings["SuperAdminDescriptorUrl"]; }
        }
          
        public static string SplashApiKey
        {
            get { return ConfigurationManager.AppSettings["SplashApiKey"]; }
        }

        public static string MerchantLoginId
        {
            get { return ConfigurationManager.AppSettings["MerchantLoginId"]; }
        }

        public static string SplashMerchantEntityId
        {
            get { return ConfigurationManager.AppSettings["SplashMerchantEntityId"]; }
        }
         

        public static string SplashApiUrl
        {
            get { return ConfigurationManager.AppSettings["SplashApiUrl"]; }
        }

        public static string SplashMerchantId
        {
            get { return ConfigurationManager.AppSettings["SplashMerchantId"]; }
        }
    }
}
