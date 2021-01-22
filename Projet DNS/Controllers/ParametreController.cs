using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data.Models;
using System.Net;
using System.Net.NetworkInformation;

namespace Projet_DNS.Controllers
{
    public class ParametreController : Controller
    {

        private IApplicationLifetime ApplicationLifetime { get; set; }

        public ParametreController(IApplicationLifetime appLifetime)
        {
            ApplicationLifetime = appLifetime;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult deleteCacheDns()
        {
            if (Data.refreshDNS())
                ViewBag.deleteCache = true;
            else
                ViewBag.deleteCache = false;

            return View("Index");
        }

        [HttpPost]
        public ActionResult restartServeurDNS()
        {
            if (Data.refreshServeurUnbound())
                ViewBag.ServerRestart = true;
            else
                ViewBag.ServerRestart = false;

            return View("Index");
        }

        [HttpPost]
        public ActionResult restartAll()
        {
            // Redemarrer le Dns et supprimer le cache
            Data.refreshDNS();
            Data.refreshServeurUnbound();

            //restart le cache web
            ApplicationLifetime.StopApplication();

            return View("Index");
        }
    }
}
