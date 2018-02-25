using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using CheckDocument.Models;

namespace CheckDocument.Controllers
{
    public class DownloadController : Controller
    {

        CheckManager store = new CheckManager("STORE");
        // GET: Download
        public ActionResult Index()
        {
            Infrastructure.Logger.Info("Entering Download/Index...");
            var files = store.GetFiles("*.req").Select(name => new FileViewModel { FileName = name });
            return View(files);
        }

        public ActionResult File(string id)
        {
            Infrastructure.Logger.Info("Entering Download/File/{0}...", id);
            id = id + ".req";
            try
            {
                //copy given file to user's directory
                string src = store.GetFilePath(id);
                string dst = Path.Combine(
                    Infrastructure.GetUsersDirectory(User.Identity.Name), id);

                System.IO.File.Copy(src, dst);
                ViewBag.File = id;
                return View();
            }
            catch (Exception ex)
            {
                Infrastructure.Logger.Error(ex.ToString());
                return RedirectToAction("Index");
            }
        }
    }
}