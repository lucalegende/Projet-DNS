using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Projet_DNS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            ViewBag.HomeData = new List<int>();
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cookies = Request.Cookies[".ASPNetCore.Cookies"];

            if (cookies != null)
            {
                //Chart Data
                var chart = new List<int>();
                var dtEx = DateTime.Today;

                for (var dt = DateTime.Today; dt <= DateTime.Today.AddDays(1); dt = dt.AddHours(0.5))
                {
                    var QueryNew = Data.querylogs.Where(x => x.DateTime >= dtEx).Where(x => x.DateTime <= dt).ToList();
                    chart.Add(QueryNew.Count);
                    dtEx = dt;
                }

                ViewBag.chartValue = chart;

                //Query log
                int TotalQuery = Data.querylogs.Count();
                int TotalQueryBlocked = Data.querylogs.Where(x => x.Blacklist == true).Count();

                ViewBag.TotalQuery = TotalQuery;
                ViewBag.TotalQueryBlocked = TotalQueryBlocked;
                if (TotalQuery != 0)
                    ViewBag.PourcentBloquer = TotalQueryBlocked * 100 / TotalQuery;
                else
                    ViewBag.PourcentBloquer = 0;
                ViewBag.TotalBlacklist = Data.blacklists.Count();
                return View();
            }
            else
                return RedirectToAction("Index", "Identification");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
