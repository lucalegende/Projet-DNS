using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Projet_DNS.Controllers
{
    public class WhitelistController : Controller
    {
        // GET: WhitelistController
        public ActionResult Index()
        {
            ViewBag.Whitelist = Data.whitelists;
            return View();
        }

        [HttpPost]
        public ActionResult Add(Whitelist whitelist)
        {
            ViewBag.Domain = whitelist.Domain;
            Whitelist w = Data.whitelists.Find(x => x.Domain.Equals(whitelist.Domain));
            Blacklist b = Data.blacklists.Find(x => x.Domain.Equals(whitelist.Domain));

            if (b != null)
                ViewBag.AjoutReussi = "blackcontient";
            else if (w != null)
                ViewBag.AjoutReussi = "contient";
            else
                try
                {
                    // Vérifie si le site web existe
                    Dns.GetHostEntry(whitelist.Domain);
                    if (Data.addWhitelist(whitelist.Domain))
                        ViewBag.AjoutReussi = "reussi";
                    else
                        ViewBag.AjoutReussi = "echouer";
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(whitelist.Domain))
                    {
                        //Vérification domain et sous domaine est un .* ou pas
                        if (whitelist.Domain.Equals(".") || Data.TLD.Contains(whitelist.Domain.ToUpper()))
                        {
                            if (Data.addWhitelist(whitelist.Domain))
                                ViewBag.AjoutReussi = "reussi";
                            else
                                ViewBag.AjoutReussi = "echouer";
                        }
                        else
                            ViewBag.AjoutReussi = "echouer";
                    }
                    else
                        ViewBag.AjoutReussi = "echouer";
                }

            ViewBag.Whitelist = Data.whitelists;
            return View("Index");
        }

        [HttpPost]
        public ActionResult Delete(Whitelist whitelist)
        {
            if (Data.deleteWhitelist(whitelist.Domain))
                ViewBag.DeleteReussi = "reussi";
            else
                ViewBag.DeleteReussi = "echouer";

            ViewBag.DomainDelete = whitelist.Domain;
            ViewBag.Whitelist = Data.whitelists;

            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> RefreshAsync()
        {
            await Data.refreshWhitelist();
            ViewBag.Whitelist = Data.whitelists;

            return View("Index");
        }
    }
}
