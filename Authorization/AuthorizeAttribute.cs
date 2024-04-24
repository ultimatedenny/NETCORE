using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection.PortableExecutable;
using System;
using NETCORE.Models;

namespace WebApi.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return;
            }
            var serviceProvider = context.HttpContext.RequestServices;
            var _cookieHelper = serviceProvider.GetRequiredService<CookieHelper>();
            var _jwtUtils = serviceProvider.GetRequiredService<IJwtUtils>();
            var token = _cookieHelper.GetCookies("TOKEN");
            var userInfo = _jwtUtils.ValidateJwtToken(token);
            if (userInfo == null)
            {
                context.Result = new JsonResult(new {status = false, message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
        }
    }
}
