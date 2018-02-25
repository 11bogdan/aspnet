using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Activities.Presentation;
using CheckDocument.Models;

namespace CheckDocument.Controllers
{
    public class AdminController : Controller
    {

        private ApplicationDbContext dbManager;

        public AdminController()
        {
            dbManager = new ApplicationDbContext();
        }


        // GET: Admin
        public ActionResult Index()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            

            // first we create Admin rool   
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            string id = UserManager.Users.First().Id;
            Infrastructure.Logger.Info("Assigning to {0}", id);
            var result1 = UserManager.AddToRole(id, "Admin"); // make me an admin!


            return View();
        }
        
    }
}