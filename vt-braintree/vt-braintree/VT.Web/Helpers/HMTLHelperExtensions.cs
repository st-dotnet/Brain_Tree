﻿using System;
using System.Linq;
using System.Web.Mvc;

namespace VT.Web.Helpers
{
    public static class HmtlHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            const string cssClass = "active";
            var currentAction = (string)html.ViewContext.RouteData.Values["action"];
            var currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
            {
                action = currentAction;
            }
            else
            {
                if (action.Contains("|"))
                {
                    var actionArr = action.Split('|');
                    return controller == currentController && actionArr.Contains(currentAction) ? cssClass : String.Empty;
                }
            }

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            var currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

	}
}