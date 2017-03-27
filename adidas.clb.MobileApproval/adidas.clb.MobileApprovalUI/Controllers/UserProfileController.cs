﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using adidas.clb.MobileApprovalUI.Models;
using adidas.clb.MobileApprovalUI.Utility;
using Newtonsoft.Json;

namespace adidas.clb.MobileApprovalUI.Controllers
{
    // [Authorize]
    public class UserProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private string graphResourceID = ConfigurationManager.AppSettings["ResourceID"];


        // GET: UserProfile
        public async Task<ActionResult> Index()
        {
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                Uri servicePointUri = new Uri(graphResourceID);
                Uri serviceRoot = new Uri(servicePointUri, tenantID);
                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                      async () => await GetTokenForApplication());

                // use the token for querying the graph to get the user details

                var result = await activeDirectoryClient.Users
                    .Where(u => u.ObjectId.Equals(userObjectID))
                    .ExecuteAsync();
                IUser user = result.CurrentPage.ToList().First();

                return View(user);
            }
            catch (AdalException)
            {
                // Return to error page.
                return View("Error");
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception)
            {
                return View("Relogin");
            }
        }

        // GET: UserProfile
        public async Task<ActionResult> LogedinUser()
        {
            string tenantId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                Uri servicePointUri = new Uri("https://graph.windows.net");
                Uri serviceRoot = new Uri(servicePointUri, tenantId);
                LoggerHelper.WriteToLog("token for reading user info", CoreConstants.Priority.High, CoreConstants.Category.Trace);
                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                      async () => await GetTokenForUser());
                LoggerHelper.WriteToLog("activedirectory", CoreConstants.Priority.High, CoreConstants.Category.Trace);
                //// use the token for querying the graph to get the user details
                var result = await activeDirectoryClient.Users.Where(u => u.ObjectId.Equals(userObjectID)).ExecuteAsync();
                IUser userobj = result.CurrentPage.ToList().First();
                LoggerHelper.WriteToLog("User name" + userobj.DisplayName, CoreConstants.Priority.High, CoreConstants.Category.Trace);
                return Json(userobj, JsonRequestBehavior.AllowGet);
            }
            catch (AdalException)
            {
                // Return to error page.
                return View("Error");
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception ex)
            {
                return View("Relogin");
            }
        }

        // GET: UserProfile
        //public string getUserid()
        //{
        //    LoggerHelper.WriteToLog("getting userid", CoreConstants.Priority.High, CoreConstants.Category.Trace);
        //    IUser userobj = JsonConvert.DeserializeObject<IUser>(LogedinUser().Result.ToString());
        //    LoggerHelper.WriteToLog("got userid", CoreConstants.Priority.High, CoreConstants.Category.Trace);
        //    //LoggerHelper.WriteToLog("User name"+user.DisplayName, CoreConstants.Priority.High, CoreConstants.Category.Trace);
        //    return userobj.MailNickname;

        //}

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/UserProfile" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
        public async Task<string> GetTokenForUser()
        {
            try
            {
                string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
                ClientCredential clientcred = new ClientCredential(clientId, appKey);
                // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
                AuthenticationContext authenticationContext = new AuthenticationContext(Startup.Authority, new ADALTokenCache(userObjectID));
                LoggerHelper.WriteToLog("about to acquire token", CoreConstants.Priority.High, CoreConstants.Category.Trace);
                AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync("https://graph.windows.net", clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
                LoggerHelper.WriteToLog("token recieved - " + authenticationResult.AccessToken, CoreConstants.Priority.High, CoreConstants.Category.Trace);
                return authenticationResult.AccessToken;
            }
            catch (Exception exception)
            {
                LoggerHelper.WriteToLog(exception + " - ERROR in token : "
                       + exception.ToString(), CoreConstants.Priority.High, CoreConstants.Category.Error);
                return null;
            }
        }
        public async Task<string> GetTokenForApplication()
        {
            try
            {
                string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
                ClientCredential clientcred = new ClientCredential(clientId, appKey);
                // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
                AuthenticationContext authenticationContext = new AuthenticationContext(Startup.Authority, new ADALTokenCache(userObjectID));
                LoggerHelper.WriteToLog("about to acquire token", CoreConstants.Priority.High, CoreConstants.Category.Trace);
                AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(graphResourceID, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
                LoggerHelper.WriteToLog("token recieved - " + authenticationResult.AccessToken, CoreConstants.Priority.High, CoreConstants.Category.Trace);
                return authenticationResult.AccessToken;
            }
            catch (Exception exception)
            {
                LoggerHelper.WriteToLog(exception + " - ERROR in token : "
                       + exception.ToString(), CoreConstants.Priority.High, CoreConstants.Category.Error);
                return null;
            }
        }
    }
}
