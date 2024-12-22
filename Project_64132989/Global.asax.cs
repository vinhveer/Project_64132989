using Project_64132989.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Diagnostics;

namespace Project_64132989
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalFilters.Filters.Add(new ReloadInfoFilter("Home64132989", "Login64132989", "Portal64132989"));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
                return;

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket == null)
                return;

            // Lấy danh sách role từ UserData
            string[] roles = authTicket.UserData.Split(',');

            // Sử dụng FormsIdentity và gán roles
            var formsIdentity = new System.Web.Security.FormsIdentity(authTicket);
            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(formsIdentity, roles);
        }
    }
}
