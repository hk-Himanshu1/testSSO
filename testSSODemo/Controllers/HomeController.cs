using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using testSSODemo.Models;

namespace testSSODemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Index action to show the home page
        [AllowAnonymous]
        public IActionResult Index()
        {
            _logger.LogInformation("User is authenticated: {Authenticated}", User.Identity?.IsAuthenticated);
            return View();
        }

        // Login action to trigger the SSO login process
        [HttpPost]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("User already authenticated. Redirecting to Home.");
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("User not authenticated. Redirecting to SSO login.");
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        // Logout action to sign out the user
        [HttpPost]
        public IActionResult Logout()
        {
            _logger.LogInformation("User is logging out...");

            var callbackUrl = Url.Action(nameof(Index), "Home", null, Request.Scheme);

            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectDefaults.AuthenticationScheme,
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Error action for handling errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
