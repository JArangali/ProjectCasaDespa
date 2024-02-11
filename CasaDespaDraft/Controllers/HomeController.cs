using CasaDespaDraft.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CasaDespaDraft.Controllers
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
            return View();
        }

        public IActionResult IndexEdit()
        {
            return View();
        }

        public IActionResult EditHome() { 
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult FAQs()
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