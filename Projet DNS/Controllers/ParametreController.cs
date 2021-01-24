using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data;
using Projet_DNS.Core.Data.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Projet_DNS.Controllers
{
    public class ParametreController : Controller
    {
        private readonly DefaultContext _context = null;

        private IApplicationLifetime ApplicationLifetime { get; set; }

        public ParametreController(IApplicationLifetime appLifetime, DefaultContext context)
        {
            _context = context;
            ApplicationLifetime = appLifetime;
        }

        public IActionResult Index()
        {
            var cookies = Request.Cookies[".ASPNetCore.Cookies"];

            if (cookies != null)
                return View();
            else
                return RedirectToAction("Index", "Identification");
        }

        [HttpPost]
        public async Task<IActionResult> changePassword(string email, string password, string newPassword)
        {
            var query = from item in this._context.Profiles
                        select item;

            try
            {
                Profile Account = query.ToList().Find(x => x.EmailFirst == email && x.Password == Hash.cryptPassword(password));
                Account.Password = Hash.cryptPassword(newPassword);

                _context.Update(Account);
                await _context.SaveChangesAsync();

                ViewBag.changePassword = true;
            }
            catch (Exception e)
            {
                ViewBag.changePassword = false;
            }
            return View("Index");
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
