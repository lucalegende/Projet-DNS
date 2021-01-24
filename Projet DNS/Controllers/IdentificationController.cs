using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projet_DNS.BusinessLogic;
using Projet_DNS.Core.Data;
using Projet_DNS.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projet_DNS.Controllers
{
    public class IdentificationController : Controller
    {
        private readonly DefaultContext _context = null;

        public IdentificationController(DefaultContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            //Requete bdd
            var query = from item in this._context.Profiles
                        select item;

            if (query.ToList().Count > 0 )
            {
                var cookies = Request.Cookies[".ASPNetCore.Cookies"];

                if (cookies != null)
                    return Redirect("/Home");
                else
                    return View();
            }
            else
                return Redirect("/Installation");
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password, string ReturnUrl)
        {
            //Requete bdd
            var query = from item in this._context.Profiles
                        select item;

            if (query.ToList().Find(x => x.EmailFirst.Equals(username) && x.Password.Equals(Hash.cryptPassword(password))) != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Login");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                    });
                return Redirect(ReturnUrl == null ? "/Home" : ReturnUrl);
            }
            else
            {
                ViewBag.Erreur = "Nous n'avons trouver aucun compte avec cette email et ce mot de passe";
                return View();
            } 
        }
    }
}
