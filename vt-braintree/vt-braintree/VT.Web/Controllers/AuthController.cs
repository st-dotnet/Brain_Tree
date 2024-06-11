using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using VT.Common;
using VT.Common.Utils;
using VT.Data;
using VT.Web.Components;
using VT.Web.Components.Security;
using VT.Web.Interfaces;
using VT.Web.Models;

namespace VT.Web.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        #region Fields

        private readonly IUserAuthenticator _userAuthenticator;

        #endregion
        
        #region Constructor

        public AuthController(IUserAuthenticator userAuthenticator)
        {
            _userAuthenticator = userAuthenticator;
        }

        #endregion

        #region Action Method(s)

        [AllowAnonymous]
        [Route("~/Login")]
        public ActionResult Login(string returnUrl)
        {
            var model = new SignInViewModel
            {
                ReturnUrl = returnUrl
            };
            return View();
        }
        
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("~/Authenticate", Name = "UserLogin")]
        public ActionResult Authenticate(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                LoginResult loginResult = null;

                loginResult =  _userAuthenticator.AuthenticateUser(model.UserName, model.Password);

                if (loginResult.Success)
                {
                    // Login was successful
                    // Do post-login stuff like setting cookies, etc.

                    var roles = new List<string> { loginResult.Role };

                    // Create the authentication ticket
                    var data = new FormsAuthenticationTicketData
                    {
                        UserId = loginResult.User.CompanyWorkerId, 
                        FullName = string.Format("{0} {1}", loginResult.User.FirstName, loginResult.User.LastName),
                        EmailAddress = loginResult.User.Email,
                        Roles = roles.ToArray(),
                        CompanyId = loginResult.User.CompanyId,
                        CompanyName = loginResult.User.Company != null ? loginResult.User.Company.Name : string.Empty,
                        HasMerchantAccount = loginResult.User.Company != null && !string.IsNullOrEmpty(loginResult.User.Company.MerchantAccountId),
                        HasGatewayCustomer = loginResult.User.Company != null && !string.IsNullOrEmpty(loginResult.User.Company.GatewayCustomerId),
                        PaymentGateway = (int)loginResult.Gateway
                        
                    };

                    // Encrypt and store the auth ticket to a user cookie
                    var json = JsonConvert.SerializeObject(data);
                    var ticket = new FormsAuthenticationTicket(1,model.UserName, DateTime.UtcNow,DateTime.UtcNow.AddHours(2), true, json);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                    Response.Cookies.Add(cookie);

                    Session["User"] = model.UserName;

                    if (model.RememberMe)
                    {
                        // Remember me was checked so cookie the username
                        var usernameCookie = new HttpCookie("username", model.UserName)
                        {
                            Expires = DateTime.UtcNow.AddMonths(1)
                        };
                        Response.Cookies.Remove("username");
                        Response.Cookies.Set(usernameCookie);
                    }
                    else
                    {
                        //Remember me disabled; deleting username cookie
                        var uc = Request.Cookies["username"];
                        if (uc != null)
                        {
                            uc.Expires = DateTime.UtcNow.AddDays(-1);
                            Response.Cookies.Set(uc);
                        }
                    }

                    var success = true;
                    var message = string.Empty;

                    var redirectUrl = model.ReturnUrl.Or(Url.Action("Index", "Organizations"));

                    if (loginResult.Role == UserRoles.CompanyAdmin.ToString())
                    {
                        redirectUrl = model.ReturnUrl.Or(Url.Action("Index", "Users"));
                    }
                    if (loginResult.Role == UserRoles.CompanyUser.ToString())
                    {
                        success = data.HasGatewayCustomer && data.HasMerchantAccount;
                        message = !success ? "Your company account is not setup yet. Please contact company admin." : string.Empty;
                        redirectUrl = model.ReturnUrl.Or(Url.Action("Index", "CompanyUser"));
                    }
                    //Login complete 
                    return Json(new
                    {
                        success = success,
                        message = message,
                        redirectUrl = redirectUrl
                    });
                }
                return Json(new
                {
                    success = false,
                    message = loginResult.Message
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = string.Empty
                });
            }
        }

        [Route("~/Logout")]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Clear();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        #endregion
    }
}