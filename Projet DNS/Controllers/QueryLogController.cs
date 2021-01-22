using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data.Models;
using System.Linq;

namespace Projet_DNS.Controllers
{
    public class QueryLogController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.QueryLogs = Data.querylogs.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddBlackList(QueryLog Log)
        {
            Data.addBlacklistViaAutre(Log.Domain);

            ViewBag.QueryLogs = Data.querylogs.ToList();

            return View("Index");
        }

        [HttpPost]
        public ActionResult AddWhiteList(QueryLog Log)
        {
            Data.addWhitelistViaAutre(Log.Domain);

            ViewBag.QueryLogs = Data.querylogs.ToList();

            return View("Index");
        }
    }
}
