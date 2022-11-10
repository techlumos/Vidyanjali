using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using Quartz;
using Quartz.Impl;
using Vidyanjali.App_Start;
using Vidyanjali.Areas.EMS.Models;
using Vidyanjali.Comman;
using Vidyanjali.Models;
using System.Web.Security;
using System.Security.Principal;

namespace Vidyanjali
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<CoreContext>(null);
            //Database.SetInitializer(new SeedData());
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new SecurityFilter());
            RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalFilters.Filters.Add(new UserTrackingActionFilterAttribute());
            filters.Add(new UserTrackingActionFilterAttribute());
        }

        //public void Application_AuthenticateRequest(Object src, EventArgs e)
        //{
        //    if (!(HttpContext.Current.User == null))
        //    {
        //        if (HttpContext.Current.User.Identity.AuthenticationType == "Forms")
        //        {
        //            System.Web.Security.FormsIdentity id;
        //            id = (System.Web.Security.FormsIdentity)HttpContext.Current.User.Identity;
        //            String[] myRoles = new String[2];
        //            myRoles[0] = "Manager";
        //            myRoles[1] = "Admin";
        //            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(id, myRoles);
        //        }
        //    }
        //}

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            // retrieve roles from UserData
            string[] roles = authTicket.UserData.Split(';');

            if (Context.User != null)
                Context.User = new GenericPrincipal(Context.User.Identity, roles);
        }
    }
}