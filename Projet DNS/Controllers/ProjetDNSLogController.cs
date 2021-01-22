using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Projet_DNS.Controllers
{
    public class ProjetDNSLogController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.QueryLogFull = Data.returnLog(70);
            return View();
        }
    }
}
