using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RFQWebApp.Models;

namespace RfQWebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            Database.SetInitializer<RfQDBContext>(null);

            // Code that runs on application startup
            //Application["Totaluser"] = 0;
            //to check how many users have currently opened our site write the following line
            //Application["OnlineUserCounter"] = 0;
        }

        //protected void Session_Start()
        //{
        //    // Code that runs when a new session is started
        //    Application.Lock();
        //    Application["Totaluser"] = (Int32)Application["Totaluser"] + 1;

        //    //to check how many users have currently opened our site write the following line
        //    //Application["OnlineUserCounter"] = Convert.ToInt32(Application["OnlineUserCounter"]) + 1;
        //    Application.UnLock();
        //}

        //protected void Session_End(object sender, EventArgs e)
        //{
        //    // Code that runs when a session ends.
        //    // Note: The Session_End event is raised only when the sessionstate mode
        //    // is set to InProc in the Web.config file. If session mode is set to StateServer
        //    // or SQLServer, the event is not raised.
        //    Application.Lock();
        //    Application["OnlineUserCounter"] = Convert.ToInt32(Application["OnlineUserCounter"]) - 1;
        //    Application.UnLock();
        //}
    }
}