using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace VT.Web.Components.CustomFilter
{
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string redirectTo = "~/Logout";

            HttpContext context = HttpContext.Current;
 
            if (context != null && context.Session != null)
            {
                var user = context.Session["User"];
                if (user == null)
                { 

                    bool isAjaxCall = string.Equals("XMLHttpRequest", context.Request.Headers["x-requested-with"], StringComparison.OrdinalIgnoreCase);

                    if (isAjaxCall)
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 200;
                        context.Response.Write(
                            new JavaScriptSerializer()
                                .Serialize(
                                    new
                                    {
                                        success = false,
                                        message = "logout"
                                    }
                                )
                            );
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult(redirectTo);
                    }
                    return;
                }
            } 

            base.OnActionExecuting(filterContext);
        }
    }
}