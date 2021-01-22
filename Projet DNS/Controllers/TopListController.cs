using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projet_DNS.Controllers
{
    public class TopListController : Controller
    {
        public IActionResult Index()
        {
            //Liste des hists de site rqaueter
            ViewBag.Toplists = Data.toplists.OrderByDescending(x => x.Hits).Take(10).ToList();

            //Liste des hits de site bloquer requeter
            ViewBag.ToplistsBlocked = Data.toplists.FindAll(x => x.blocked == true).OrderByDescending(x => x.Hits).Take(10).ToList();

            return View();
        }
    }
}
