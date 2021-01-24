using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Projet_DNS.Controllers
{
    public class GraphiqueController : Controller
    {
        public IActionResult Index()
        {
            var cookies = Request.Cookies[".ASPNetCore.Cookies"];

            if (cookies != null)
            {
                //Gaphics pour les requetes
                List<Toplist> chartDonut = Data.toplists.OrderByDescending(x => x.Hits).Take(10).ToList();
                ViewBag.chartDonutLabel = new List<string>();
                ViewBag.chartDonutData = new List<int>();

                foreach (Toplist item in chartDonut)
                {
                    ViewBag.chartDonutLabel.Add(item.Domain);
                    ViewBag.chartDonutData.Add(item.Hits);
                }

                //Graphique requete bloquer
                List<Toplist> chartPie = Data.toplists.FindAll(x => x.blocked == true).OrderByDescending(x => x.Hits).Take(10).ToList();
                ViewBag.chartPieLabel = new List<string>();
                ViewBag.chartPieData = new List<int>();

                foreach (Toplist item in chartPie)
                {
                    ViewBag.chartPieLabel.Add(item.Domain);
                    ViewBag.chartPieData.Add(item.Hits);
                }

                return View();
            }
            else
                return RedirectToAction("Index", "Identification");
        }
    }
}
