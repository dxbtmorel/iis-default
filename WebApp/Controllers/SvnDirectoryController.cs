using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class DirectoryController : Controller
    {
        public ActionResult Dev(SvnDirectoryType type)
        {
            ViewBag.Title = "Dev";
            
            return PartialView("List", GetSvnDirectories(type));
        }

        public ActionResult Stable(SvnDirectoryType type)
        {
            ViewBag.Title = "Stable";

            return PartialView("List", GetSvnDirectories(type));
        }

        public ActionResult Old(SvnDirectoryType type)
        {
            ViewBag.Title = "Old";

            return PartialView("List", GetSvnDirectories(type));
        }

        public ActionResult Project(SvnDirectoryType type)
        {
            ViewBag.Title = "Project";

            return PartialView("List", GetSvnDirectories(type));
        }

        public ActionResult Edit(SvnDirectory svnDirectory)
        {
            ViewBag.Title = "Edit";

            return PartialView(svnDirectory);
        }

        protected List<SvnDirectory> GetSvnDirectories(SvnDirectoryType type)
        {
            List<SvnDirectory> listSvnDirectories = new List<SvnDirectory>();

            string physicalPath = Server.MapPath("/");
            string[] directories = Directory.GetDirectories(physicalPath, "buyer*");

            foreach (string directory in directories)
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                SvnDirectory svnDirectory = new SvnDirectory()
                {
                    Name = di.Name
                };

                if (svnDirectory.Type == type)
                    listSvnDirectories.Add(svnDirectory);
            }

            return listSvnDirectories;
        }
    }
}