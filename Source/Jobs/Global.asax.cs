using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Data.Models;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Schedules.SchedulerClasses;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web.Caching;

namespace Jobs
{
    public class MvcApplication : System.Web.HttpApplication
    {
		private const string DummyCacheItemKey = "GagaGuguGigi";
        protected void Application_Start()
        {
             //ApplicationDbContext context = new Data.Models.ApplicationDbContext();
             //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
             //var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Scheduler.Start();

            RegisterCacheEntry();
            //SqlDependency.Start(ConfigurationManager.ConnectionStrings["LoginDB"].ToString());
        }

        public static ApplicationDbContext ApplicationDbContext
        {
            get
            {
                return DependencyResolver.Current.GetService<ApplicationDbContext>();
            }
        }

        public override void Init()
        {
            base.Init();
            try
            {
                // Get the app name from config file...
                string appName = ConfigurationManager.AppSettings["ApplicationName"];

                if (string.IsNullOrEmpty(appName))
                {
                    throw new Exception("Application Name is not set in Jobs Project");
                }
                else if (!string.IsNullOrEmpty(appName))
                {
                    foreach (string moduleName in this.Modules)
                    {
                        IHttpModule module = this.Modules[moduleName];
                        SessionStateModule ssm = module as SessionStateModule;
                        if (ssm != null)
                        {
                            FieldInfo storeInfo = typeof(SessionStateModule).GetField("_store", BindingFlags.Instance | BindingFlags.NonPublic);
                            SessionStateStoreProviderBase store = (SessionStateStoreProviderBase)storeInfo.GetValue(ssm);
                            if (store == null) //In IIS7 Integrated mode, module.Init() is called later
                            {
                                FieldInfo runtimeInfo = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.Static | BindingFlags.NonPublic);
                                HttpRuntime theRuntime = (HttpRuntime)runtimeInfo.GetValue(null);
                                FieldInfo appNameInfo = typeof(HttpRuntime).GetField("_appDomainAppId", BindingFlags.Instance | BindingFlags.NonPublic);
                                appNameInfo.SetValue(theRuntime, appName);
                            }
                            else
                            {
                                Type storeType = store.GetType();
                                if (storeType.Name.Equals("OutOfProcSessionStateStore"))
                                {
                                    FieldInfo uribaseInfo = storeType.GetField("s_uribase", BindingFlags.Static | BindingFlags.NonPublic);
                                    uribaseInfo.SetValue(storeType, appName);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        ///////////////////////////////////////

        private bool RegisterCacheEntry()
        {

            if (null != HttpContext.Current.Cache[DummyCacheItemKey]) return false;


            HttpContext.Current.Cache.Add(DummyCacheItemKey, "Test", null,
                DateTime.MaxValue, TimeSpan.FromMinutes(2),
                CacheItemPriority.Normal,
                new CacheItemRemovedCallback(CacheItemRemovedCallback));

            return true;
        }
        public void CacheItemRemovedCallback(string key,
            object value, CacheItemRemovedReason reason)
        {
            // Debug.WriteLine("Cache item callback: " + DateTime.Now.ToString());

            // Do the service works


            HitPage();
        }


        //private const string DummyPageUrl = "http://192.168.2.110:81/";
        private const string DummyPageUrl = "https://localhost:44305/";

        private void HitPage()
        {

            WebClient client = new WebClient();
            client.DownloadData(DummyPageUrl);
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // If the dummy page is hit, then it means we want to add another item

            // in cache

            if (HttpContext.Current.Request.Url.ToString() == DummyPageUrl)
            {
                // Add the item in cache and when succesful, do the work.
                DoSomeFileWritingStuff();
                RegisterCacheEntry();

            }
        }

        private void DoSomeFileWritingStuff()
        {
            Debug.WriteLine("Writing to file...");

            try
            {
                using (StreamWriter writer =
                 new StreamWriter(@"c:\temp\Cachecallback.txt", true))
                {
                    writer.WriteLine("Cache Callback: {0}", DateTime.Now);
                    writer.Close();
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
            }

            Debug.WriteLine("File write successful");
        }




    }
}
