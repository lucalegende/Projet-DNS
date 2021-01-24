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
            var cookies = Request.Cookies[".ASPNetCore.Cookies"];

            if (cookies != null)
            {
                ViewBag.QueryLogFull = Data.returnLog(70);
                return View();
            }
            else
                return RedirectToAction("Index", "Identification");
        }
    }
}
