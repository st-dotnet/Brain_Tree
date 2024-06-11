using System;
using System.Security.Policy;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Newtonsoft.Json;
using VT.Common;
using VT.Web.Components.Security;

namespace VT.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutfacConfig.Setup();
            AutomapConfig.Setup();

            EO.Pdf.Runtime.AddLicense(
    "5sAEFOan2PgGHeR35dXhBc+m7PfS3cKq3PTfJLix8Of/7Lx2s7MEFOan2PgG" +
    "HeR3hI7N2uui2un/HuR3hI514+30EO2s3MKetZ9Zl6TNF+ic3PIEEMidtbvJ" +
    "27hurrTD3bB1pvD6DuSn6unaD71GgaSxy5914+30EO2s3OnP566l4Of2GfKe" +
    "3MKetZ9Zl6TNDOul5vvPuIlZl6Sxy59Zl8DyD+NZ6/0BELxbvNO/++OfmaQH" +
    "EPGs4PP/6KFqrLLBzZ9otZGby59Zl8DADOul5vvPuIlZl6Sx5+6r2+kD9O2f" +
    "5qT1DPOetKbC3a5qrbPD27BumaQEIOF+7/T6HeSsuPjOzbBrprXH2rFpqLqz" +
    "y/We6ff6Gu12mbXGzZ9otZGby59Zlw==");

        }

        protected void Application_BeginRequest()
        {
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            //Response.Cache.SetNoStore();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            // Get the cookie
            var cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

            // Nothing to do if no cookie
            if (cookie == null)
                return;

            // Decrypt the cookie
            var data = FormsAuthentication.Decrypt(cookie.Value);

            // Nothing to do if null
            if (data == null)
                return;

            // Deserialize the custom data we stored in the cookie
            var o = JsonConvert.DeserializeObject<FormsAuthenticationTicketData>(data.UserData);

            // Nothing to do if null
            if (o == null)
                return;

            // Use the custom data to recreate the identity and principal
            var genericIdentity = new CustomIdentity(o.EmailAddress, o.UserId, o.FullName, o.Roles[0], 
                o.CompanyId, o.CompanyName, o.HasMerchantAccount, o.HasGatewayCustomer, o.PaymentGateway);

            var genericPrincipal = new GenericPrincipal(genericIdentity, o.Roles);

            // Assign to current user request
            HttpContext.Current.User = genericPrincipal;

            // Assign to current thread as well
            Thread.CurrentPrincipal = genericPrincipal;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Get last error
            var exception = Server.GetLastError();

            if (exception == null)
                return;

            //Send email
            //SendEmail(exception.GetBaseException());

            //Log error in the database
            //LogManager.AddLog(exception.ToString(), LogEntryType.Error, LogSection.AmutaAdmin);

            //Check if it was ajax request and return response accordingly
            bool isAjaxCall = string.Equals("XMLHttpRequest", Context.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);

            var httpException = exception as HttpException;

            //Server.ClearError();

            if (isAjaxCall)
            {
                Context.Response.ContentType = "application/json";
                Context.Response.StatusCode = 200;
                Context.Response.Write(
                    new JavaScriptSerializer()
                    .Serialize(
                        new
                        {
                            success = false,
                            message = "Server internal error"
                        }
                      )
                    );
            }
            else
            {
                var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

                if (exception.ToString().Contains("CustomIdentity"))
                {
                    var loginUrl = url.Action("Login", "Auth",
                        new {id = (httpException != null) ? httpException.GetHttpCode() : 0});
                    if (loginUrl != null) Response.Redirect(loginUrl);
                }
                var errorUrl = url.Action("Index", "Error", new { id = (httpException != null) ? httpException.GetHttpCode() : 0 });
                if (errorUrl != null) Response.Redirect(errorUrl);
            }
        }

        protected void Session_OnEnd(object sender, EventArgs e)
        {
        }
    }
}
