using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Projet_DNS.Controllers
{
    public class BlacklistController : Controller
    {
        #region Méthodes
        // GET: BlacklistController
        public ActionResult Index()
        {
            var cookies = Request.Cookies[".ASPNetCore.Cookies"];

            if (cookies != null)
            {
                ViewBag.Blacklist = Data.blacklists;
                return View();
            }  
            else
                return RedirectToAction("Index", "Identification");
        }

        [HttpPost]
        public ActionResult Add(Blacklist blacklist)
        {
            ViewBag.Domain = blacklist.Domain;
            Blacklist b = Data.blacklists.Find(x => x.Domain.Equals(blacklist.Domain));
            Whitelist w = Data.whitelists.Find(x => x.Domain.Equals(blacklist.Domain));

            if (w != null)
                ViewBag.AjoutReussi = "whitecontient";
            else if (b != null)
                ViewBag.AjoutReussi = "contient";
            else
                try
                {
                    // Vérifie si le site web existe
                    Dns.GetHostEntry(blacklist.Domain);
                    if (Data.addBlacklist(blacklist.Domain))
                        ViewBag.AjoutReussi = "reussi";
                    else
                        ViewBag.AjoutReussi = "echouer";
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(blacklist.Domain))
                    {
                        //Vérification domain et sous domaine est un .* ou pas
                        if (blacklist.Domain.Equals(".") || Data.TLD.Contains(blacklist.Domain.ToUpper()))
                        {
                            if (Data.addBlacklist(blacklist.Domain))
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

            ViewBag.Blacklist = Data.blacklists;
            return View("Index");
        }

        [HttpPost]
        public ActionResult Delete(Blacklist blacklist)
        {
            if (Data.deleteBlacklist(blacklist.Domain))
                ViewBag.DeleteReussi = "reussi";
            else
                ViewBag.DeleteReussi = "echouer";

            ViewBag.DomainDelete = blacklist.Domain;
            ViewBag.Blacklist = Data.blacklists;

            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> RefreshAsync()
        {
            await Data.refreshBlacklist();
            ViewBag.Blacklist = Data.blacklists;

            return View("Index");
        }
        #endregion
    }
}
