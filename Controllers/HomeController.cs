using Microsoft.AspNetCore.Mvc;
using NETCORE.Models;
using System.Diagnostics;
using WebApi.Authorization;
using System.Reflection;
using NETCORE.Classes;
using System.Text;

namespace NETCORE.Controllers
{
    [CustomAuthorize]
    public class HomeController(ILogger<HomeController> logger, IConfiguration configuration, Class dataClass, CookieHelper cookieHelper, IJwtUtils jwtUtils) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly Class _dataClass = dataClass;
        private readonly CookieHelper _cookieHelper = cookieHelper;
        private readonly IJwtUtils _jwtUtils = jwtUtils;
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Index()
        {
            try
            {
                GetAppInfo();
                GetUserInfo();
                ViewBag.Title = "Index";
                return View();
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.ActionDescriptor.ActionName;
                _logger.LogError(ex, $"An error occurred in while processing the {actionName} data.");
                return StatusCode(500, new { error = $"Internal server error in {actionName}. Please try again later or contact support." });
            }
        }
        public IActionResult Privacy()
        {
            GetAppInfo();
            GetUserInfo();
            return View();
        }
        public IActionResult About()
        {
            GetAppInfo();
            GetUserInfo();
            return View();
        }
        public void GetAppInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string versions = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "Unknown";
            string appNames = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Unknown";
            string companys = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown";
            string latestBuild = (System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)).ToString("dd MMM yyyy HH:mm:ss tt").ToUpper();
            string headers = $"{appNames} © {DateTime.Now.Year} {companys}";
            var netCoreVer = Environment.Version.ToString();
            var runtimeVer = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            AppInfo appInfo = new()
            {
                Version = versions,
                Name = appNames,
                LatestBuild = latestBuild,
                Company = companys,
                Footer = headers,
                DotNetVersion = netCoreVer,
                RuntimeVersion = runtimeVer
            };
            ViewBag.AppInfo = appInfo;
        }
        public void GetUserInfo()
        {
            var token    = _cookieHelper.GetCookies("TOKEN");
            var userInfo = _jwtUtils.ValidateJwtToken(token);
            var INITIALS = GetFirstAndLastInitials(userInfo.Name);
            ViewBag.userInfo = userInfo;
            ViewBag.INITIALS = INITIALS;
        }
        static string GetFirstAndLastInitials(string fullName)
        {
            string[] words = fullName.Split(' ');
            StringBuilder initialsBuilder = new();
            if (words.Length > 0 && !string.IsNullOrEmpty(words[0]))
            {
                initialsBuilder.Append(words[0][0]);
            }
            if (words.Length > 1 && !string.IsNullOrEmpty(words[^1]))
            {
                initialsBuilder.Append(words[^1][0]);
            }
            string initials = initialsBuilder.ToString().ToUpper();
            return initials;
        }
    }
}
