using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TalentLink.Models;

namespace TalentLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Client"))
                {
                    return RedirectToAction("Dashboard", "Clients");
                }
                else if (User.IsInRole("Freelancer"))
                {
                    return RedirectToAction("Dashboard", "Freelancers");
                }
            }

            // For visitors (not logged in), show the public landing page
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
