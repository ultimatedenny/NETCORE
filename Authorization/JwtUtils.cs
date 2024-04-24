using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NETCORE.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Helpers;

namespace WebApi.Authorization;
public interface IJwtUtils
{
    public string GenerateJwtToken(UserInfo user);
    UserInfo? ValidateJwtToken(string? token);
}

public class JwtUtils : IJwtUtils
{
    private readonly AppSettings _appSettings;

    public JwtUtils(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;

        if (string.IsNullOrEmpty(_appSettings.Secret))
            throw new Exception("JWT secret not configured");
    }

    public string GenerateJwtToken(UserInfo user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);

        var claims = new List<Claim>();

        var properties = typeof(UserInfo).GetProperties();

        foreach (var property in properties)
        {
            var value = (property.GetValue(user) ?? "").ToString();
            claims.Add(new Claim(property.Name, value));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public UserInfo? ValidateJwtToken(string? token)
    {
        if (token == null)
        {
            return null;
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var exp_date = int.Parse(jwtToken.Claims.First(x => x.Type == "exp").Value);
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(exp_date).UtcDateTime;

            var userInfo = new UserInfo
            {
                Name = jwtToken.Claims.First(x => x.Type == "Name").Value,
                Email = jwtToken.Claims.First(x => x.Type == "Email").Value,
                Department = jwtToken.Claims.First(x => x.Type == "Department").Value,
                Title = jwtToken.Claims.First(x => x.Type == "Title").Value,
                PhoneNumber = jwtToken.Claims.First(x => x.Type == "PhoneNumber").Value,
                Company = jwtToken.Claims.First(x => x.Type == "Company").Value,
                Country = jwtToken.Claims.First(x => x.Type == "Country").Value,
                Manager = jwtToken.Claims.First(x => x.Type == "Manager").Value,
                EmployeeID = jwtToken.Claims.First(x => x.Type == "EmployeeID").Value,
                Office = jwtToken.Claims.First(x => x.Type == "Office").Value,
                Token = token,
                UserID = jwtToken.Claims.First(x => x.Type == "UserID").Value,
                Password = jwtToken.Claims.First(x => x.Type == "Password").Value,
                TokenExp = dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return userInfo;
        }
        catch
        {
            return null;
        }
    }

}