using ADF.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ADF.Web.Controllers
{
    public class NetCoreController : Controller
    {
        private readonly ILogger<NetCoreController> _logger;
        public NetCoreController(ILogger<NetCoreController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            this._logger.LogWarning("begin");

            base.ViewData["User1"] = new CurrentUser()
            {
                Id = 7,
                Name = "ViewData",
                Account = " ╰つ Ｈ ♥. 花心胡萝卜",
                Email = "莲花未开时",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };

            base.ViewBag.Name = "wjc";
            base.ViewBag.User = new CurrentUser()
            {
                Id = 7,
                Name = "ViewBag",
                Account = "限量版",
                Email = "莲花未开时",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };

            base.TempData["User"] = new CurrentUser()
            {
                Id = 7,
                Name = "TempData",
                Account = "限量版",
                Email = "莲花未开时",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            };

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentUserSession")))
            {
                //base.HttpContext.Session.SetString("CurrentUserSession", JsonConvert.SerializeObject(new CurrentUser
                //{
                //    Id = 7,
                //    Name = "Session",
                //    Account = "季雨林",
                //    Email = "KOKE",
                //    Password = "落单的候鸟",
                //    LoginTime = DateTime.Now
                //}));
            }

            return View(new CurrentUser
            {
                Id = 7,
                Name = "Model",
                Account = "季雨林",
                Email = "KOKE",
                Password = "落单的候鸟",
                LoginTime = DateTime.Now
            });
        }
    }
}
