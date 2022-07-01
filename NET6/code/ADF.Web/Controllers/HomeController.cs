
using ADF.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ADF.Web.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _iConfiguration;
        private ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> loger)
        {
            _iConfiguration = configuration;
            _logger = loger;
        }

        public IActionResult Index()
        {
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
