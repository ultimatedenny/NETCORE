using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApi.Authorization;
using WebApi.Models;
using WebApi.Services;
using IconCaptcha.Exceptions;
using AllowAnonymousAttribute = WebApi.Authorization.AllowAnonymousAttribute;
using IconCaptcha;

namespace NETCORE.Controllers
{
    [CustomAuthorizeAttribute]
    public class UserController(IconCaptchaService captcha, IUserService userService, CookieHelper cookieHelper, ILogger<HomeController> logger) : Controller
    {
        private IUserService _userService = userService;
        private readonly CookieHelper _cookieHelper = cookieHelper;
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IconCaptchaService _captcha = captcha;

        [AllowAnonymous]
        public IActionResult Login()
        {
            try
            {
                var serviceProvider = HttpContext.RequestServices;
                var _cookieHelper = serviceProvider.GetRequiredService<CookieHelper>();
                var _jwtUtils = serviceProvider.GetRequiredService<IJwtUtils>();
                var token = _cookieHelper.GetCookies("TOKEN");
                if (!string.IsNullOrEmpty(token))
                {
                    var userInfo = _jwtUtils.ValidateJwtToken(token);
                    if (userInfo != null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.ActionDescriptor.ActionName;
                _logger.LogError(ex, $"An error occurred in while processing the {actionName} data.");
                return StatusCode(500, new { success = false, message = $"Internal server error in {actionName}. Please try again later or contact support." });
            }
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            var cookies = Request.Cookies.Keys;
            foreach (var cookieName in cookies)
            {
                Response.Cookies.Delete(cookieName);
            }
            return RedirectToAction("Login", "User");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            try
            {
                _captcha.ValidateSubmission();
                var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
                if (isValid)
                {
                    var result = _userService.Authenticate(model);
                    if (result != null)
                    {
                        _cookieHelper.PostCookies("TOKEN", result.Token);
                        return Json(new { success = true, message = "Authentication successful" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid credentials" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Model validation failed" });
                }
            }
            catch (IconCaptchaSubmissionException e)
            {
                return Json(new { success = false, message = e.Message });
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.ActionDescriptor.ActionName;
                string errMessage = $"An error occurred in while processing the '{actionName}' data. \n" + ex.Message;
                _logger.LogError(actionName, errMessage);
                return Json(new { success = false, message = errMessage });
            }
        }

    }
}
