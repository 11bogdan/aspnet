using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CheckDocument.Library;
using CheckDocument.Models;

namespace CheckDocument.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext dbManager;

        public UserController()
        {
            dbManager = new ApplicationDbContext();
        }

        public ActionResult Delete(string id)
        {
            CheckManager manager = new CheckManager(User.Identity.Name);

            try
            {
                string fileName = manager.GetFilePath(id);
                System.IO.File.Delete(fileName);
            }
            catch (Exception ex)
            {
                Infrastructure.Logger.Error(ex.ToString());
            }

            return RedirectToAction("Index");
        }

        // GET: User
        public ActionResult Index()
        {
            CheckManager manager = new CheckManager(User.Identity.Name);

            UserInfoViewModel m = new UserInfoViewModel()
            {
                UserLogin = "somelog",
                Files = manager.GetFiles("*")
            };
            return View(m);
        }

        public ActionResult Check()
        {
            // return user's documents and .docxreq
            CheckManager manager = new CheckManager(User.Identity.Name);

            List<string> filesToCheck = manager.GetFiles("*.docx");
            List<string> reqFiles = manager.GetFiles("*.req");

            ViewBag.FilesToCheck = filesToCheck ?? new List<string>();
            ViewBag.ReqFiles = reqFiles ?? new List<string>();

            return View();
        }

        [HttpPost]
        public ActionResult Check(FormCollection collection)
        {
            try
            {
                CheckManager checkManager = new CheckManager(User.Identity.Name);

                List<string> filesToCheck = new List<string>();
                if (String.IsNullOrEmpty(collection["filesToCheck"]))
                {
                    Infrastructure.Logger.Error("Null to check files");
                    return View();

                }
                filesToCheck.AddRange(((string)collection["filesToCheck"]).Split(','));

                List<string> reqFiles = new List<string>();
                if (String.IsNullOrEmpty(collection["reqFiles"]))
                {
                    Infrastructure.Logger.Error("Null req files");
                    return View();
                }
                reqFiles.AddRange(((string)collection["reqFiles"]).Split(','));

                reqFiles = reqFiles
                    .Select(x => checkManager.GetFilePath(x))
                    .ToList();

                filesToCheck = filesToCheck
                    .Select(x => checkManager.GetFilePath(x))
                    .ToList();

                Infrastructure.Logger.Info("to check:" + String.Join(",", filesToCheck));

                //!!! check collection values and add them

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        foreach (HttpPostedFile file in Request.Files)
                        {
                            // add this file to current user
                            string fileName =
                                Path.Combine(Infrastructure.GetUsersDirectory(User.Identity.Name), file.FileName);

                            //!!!check extension
                            file.SaveAs(fileName);
                        }
                    }

                    List<ReqFile> rqfiles = reqFiles.Select(f => new ReqFile(f)).ToList();
                    DocumentMatcher dm = new DocumentMatcher(rqfiles);

                    List<CheckResult> results = new List<CheckResult>();
                    foreach (string fileName in filesToCheck)
                    {
                        FileToCheck fcheck = new FileToCheck(fileName);
                        results.AddRange(dm.Check(fcheck));
                        Infrastructure.Logger.Info("After {0}: count is {1}", fileName, results.Count);
                    }

                    Infrastructure.Logger.Info("Message: " + results[0].ToString());

                    var model = new CheckResultModel()
                    {
                        Message = results[0].ToString().Split('\n').ToList(),
                        ReqFile = Path.GetFileName(reqFiles[0]),
                        DocxFile = Path.GetFileName(filesToCheck[0])
                    };

                    return View("CheckResultView", model);
                }
            }
            catch (Exception e)
            {
                Infrastructure.Logger.Error(e.ToString());
                return View();
            }
            return View();
        }

        public ActionResult CreateReqFile()
        {
            return View();
        }

        public ActionResult LoadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadFile(HttpPostedFileBase file)
        {
            if (!(file != null && file.ContentLength > 0))
            {
                return View("LoadFile");
            }
            try
            {
                var fileName = Path.GetFileName(file.FileName);

                //check ext
                Infrastructure.Logger.Info("Name red: " + file.ToString() + "|||" + file.FileName);
                var exts = Infrastructure.GetAllowedExts();
                var ext = Path.GetExtension(file.FileName);

                Infrastructure.Logger.Info("Comparing: {0} and {1}", String.Join(",", exts), ext);

                if (!exts.Contains(ext))
                {
                    Infrastructure.Logger.Error("Wrong!");
                    return View("LoadFile");
                }

                dbManager.Files.Add(new Files()
                {
                    UserId = User.Identity.Name,
                    FileName = fileName
                });

                dbManager.SaveChanges();

                string dir = Infrastructure.GetUsersDirectory(User.Identity.Name);
                var path = Path.Combine(dir, fileName);
                file.SaveAs(path);
            }
            catch (Exception e)
            {
                Infrastructure.Logger.Error(e.ToString());
                throw e;
            }

            Infrastructure.Logger.Info("Load succeeded!");
            return RedirectToAction("Index") ;
        }


        public ActionResult Rule(string id)
        {
            try
            {
                return PartialView("_" + id);
            }
            catch (Exception e)
            {
                Infrastructure.Logger.Error(e.ToString());
                throw e;
            }
        }

        //public ActionResult Send()
        //{
        //    var Users = new string[] { "Uasya", "Shamil" }.Select(s => new SelectListItem() { Text = s, Value = s });
        //    var FileNames = new string[] { "file.docx", "file2.req" }.Select(s => new SelectListItem() {Text = s, Value = s });

        //    ViewBag.Users = Users;
        //    ViewBag.FileNames = FileNames;

        //    return View();
        //}

        [HttpPost]
        public ActionResult Send(SendViewModel sendReq)
        {
            var target = dbManager.Users.First(u => u.UserName == sendReq.UserName);
            CheckManager checkManager = new CheckManager(target.UserName);

            string targPath = Infrastructure.GetUsersDirectory(target.UserName);
            string srcPath = Infrastructure.GetUsersDirectory(User.Identity.Name);

            System.IO.File.Copy(srcPath, targPath);

            Infrastructure.Logger.Info("{0} copied succ to {1}", srcPath, targPath);

            return View("SendSuccess");
        }

    }
}