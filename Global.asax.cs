using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using CheckDocument.Models;

namespace CheckDocument
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //some db ops
            UpdateDb();
        }

        private void UpdateDb()
        {
            using (var context = new ApplicationDbContext())
            {

                foreach (var file in context.Files)
                {
                    string fileName = Path.Combine(Infrastructure.RootDir, file.FileName);
                    if (!File.Exists(fileName))
                    {
                        context.Files.Remove(file);
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
