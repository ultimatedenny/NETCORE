
using Microsoft.Extensions.Options;
using NETCORE.Helpers;
using NETCORE.Models;
using System.Security.Cryptography;
using WebApi.Helpers;

public class CookieHelper
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;

    public CookieHelper(IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> appSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings.Value;

        if (string.IsNullOrEmpty(_appSettings.Public) || string.IsNullOrEmpty(_appSettings.Private))
        {
            throw new Exception("Key not configured");
        }
    }

    public string GetCookies(string Key)
    {;
        string Public  = _appSettings.Public ?? "";
        string Private = _appSettings.Private ?? "";
        var value = _httpContextAccessor.HttpContext.Request.Cookies[Key + "_" + Public]?.ToString().Decrypt(Private);
        return value;
    }

    public void PostCookies(string Key, string Value)
    {
        string Public  = _appSettings.Public ?? "";
        string Private = _appSettings.Private ?? "";
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        _httpContextAccessor.HttpContext.Response.Cookies.Append(Key + "_" + Public, Value.Encrypt(Private), cookieOptions);
    }

    static string GenerateRandomKey()
    {
        const int keySize = 256;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = keySize;
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }
}
