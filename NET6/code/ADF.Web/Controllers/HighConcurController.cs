using ADF.Utility;
using Microsoft.AspNetCore.Mvc;

namespace ADF.Web.Controllers
{
    public class HighConcurController : Controller
    {
        private IConfiguration _iConfiguration;
        private ILogger<HighConcurController> _logger;
        private ICustomMemoryCache _iCustomMemoryCache;
        /// <summary>
        /// 统计请求次数
        /// </summary>
        private static int _TotalCount = 0;

        public HighConcurController(IConfiguration configuration, ILogger<HighConcurController> loger, ICustomMemoryCache customMemoryCache)
        {
            _iConfiguration = configuration;
            _logger = loger;
            _iCustomMemoryCache = customMemoryCache;
        }

        [ResponseCache(Duration = 60)]
        public IActionResult Index()
        {
            #region 缓存
            //base.HttpContext.Response.Headers["Cache-Control"] = "public,max-age=60";
            base.ViewBag.Now = DateTime.Now;

            string key = $"SecondController-Info";
            if (!this._iCustomMemoryCache.TryGetValue(key, out object time))
            {
                // 若获取不到，新增
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                this._iCustomMemoryCache.Set(key, time);
            }
            base.ViewBag.CustomMemoryCacheNow = time;
            #endregion

            #region 负载均衡
            this._logger.LogWarning($"This is HighConcurController Index {this._iConfiguration["port"]}");

            //地址信息
            base.ViewBag.BrowserUrl = $"{base.Request.Scheme}://{base.Request.Host.Host}:{base.Request.Host.Port}/  ";//浏览器地址
            base.ViewBag.InternalUrl = $"{base.Request.Scheme}://{base.Request.Host.Host}:{this._iConfiguration["port"]}";//应用程序监听地址

            base.ViewBag.TotalCount = _TotalCount++;
            #endregion

            string user = base.HttpContext.Session.GetString("CurrentUser");
            if (!string.IsNullOrEmpty(user))
            {
                base.HttpContext.Session.SetString("CurrentUser", $"wjc={_iConfiguration["port"]}");
                this._logger.LogWarning($"this is HighConcurController {this._iConfiguration["port"]} Session");
            }
            base.ViewBag.SessionUser = base.HttpContext.Session.GetString("CurrentUser");

            return View();
        }
    }
}
