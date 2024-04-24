
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Authorization;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CookieHelper _cookieHelper;

    public JwtMiddleware(RequestDelegate next, CookieHelper cookieHelper)
    {
        _next = next;
        _cookieHelper = cookieHelper;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token  = _cookieHelper.GetCookies("TOKEN") ?? context.Request.Headers.Authorization.ToString().Replace("Bearer ","").Replace(" ","").Replace("\n","");
        var userId = jwtUtils.ValidateJwtToken(token);
        if (userId != null)
        {
            context.Items["User"] = userService.GetByName(userId.ToString());
        }
        await _next(context);
    }
}