using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data;
using Projet_DNS.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projet_DNS.Controllers
{
    public class InstallationController : Controller
    {
        private readonly DefaultContext _context = null;

        public InstallationController(DefaultContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //Requete bdd
            var query = from item in this._context.Profiles
                        select item;

            if (query.ToList().Count > 0)
                return Redirect("/Identification");
            else
                return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            try
            {
                Profile profile = new Profile
                {
                    Id = 1,
                    EmailFirst = email,
                    Password = Hash.cryptPassword(password)
                };

                _context.Add(profile);
                await _context.SaveChangesAsync();

                return Redirect("/Identification");
            }
            catch(Exception e)
            {
                ViewBag.Erreur = "Une erreur c'est produit lors de votre enregistrement";
            }

            return View();
        }
    }
}
