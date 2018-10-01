﻿using Microsoft.AspNetCore.Mvc;
using MvcClientSample.Models;

namespace MvcClientSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new HomeIndexViewModel()
            {
                Claims = User.Claims
            });
        }

        public IActionResult Logout()
        {
            return new SignOutResult(new[] { "Cookies", "oidc" });
        }
    }
}
