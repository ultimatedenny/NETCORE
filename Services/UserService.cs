
using Microsoft.IdentityModel.Tokens;
using NETCORE.Models;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Authorization;
using WebApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApi.Services;
public interface IUserService
{
    AuthenticateResponse? Authenticate(AuthenticateRequest model);
    UserInfo? GetByName(string Name);
}
public class UserService(IJwtUtils jwtUtils, IConfiguration configuration, ILogger<UserService> logger) : IUserService
{
    private List<UserInfo> _users = [];
    private readonly IJwtUtils _jwtUtils = jwtUtils;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<UserService> _logger = logger;
    public AuthenticateResponse? Authenticate(AuthenticateRequest model)
    {
        var result  = GetUserAD(model.Username, model.Password, "SHIMANOACE");
        var result2 = GetUserMD(model.Username);
        if (result2 != null)
        {
            _users.Add(new UserInfo
            {
                Name = result.Name ?? result2[0].UserName,
                Email = result.Email ?? result2[0].Email,
                Department = result2[0].Department ?? result.Department,
                Title = result.Title ?? result2[0].Department,
                PhoneNumber = result.PhoneNumber,
                Company = result.Company,
                Country = result.Country,
                Manager = result.Manager ?? "",
                EmployeeID = result2[0].UserId,
                Office = result.Office,
                Token = result.Token,
                UserID = model.Username,
                Password = model.Password
            });
        }
        var user = _users.SingleOrDefault(x => x.Name != null);
        if (result == null) return null;
        var token = _jwtUtils.GenerateJwtToken(user);
        return new AuthenticateResponse(user, token);
    }
    public UserInfo? GetByName(string Name)
    {
        return _users.FirstOrDefault(x => x.Name == Name);
    }
    public void JwtGenerate()
    {
        var secretKey = "2ed8111a-70b4-43fc-9f58-7b9015fd1b65";
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.Role, "Admin"),
            };
        var token = new JwtSecurityToken(
            issuer: "your_issuer",
            audience: "your_audience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Token expiration time
            signingCredentials: signingCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine("Generated Token:");
        Console.WriteLine(tokenString);
    }
    public UserInfo GetUserAD(string userName, string password, string domain)
    {
        UserInfo userInfo = new();
        using (DirectoryEntry entry = new("LDAP://" + domain, userName, password))
        {
            DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = "(&(objectClass=user)(sAMAccountName=" + userName + "))";
            searcher.PropertiesToLoad.Add("displayName");
            searcher.PropertiesToLoad.Add("mail");
            searcher.PropertiesToLoad.Add("department");
            searcher.PropertiesToLoad.Add("title");
            searcher.PropertiesToLoad.Add("telephoneNumber");
            searcher.PropertiesToLoad.Add("company");
            searcher.PropertiesToLoad.Add("co");
            //searcher.PropertiesToLoad.Add("manager"); // User's manager
            //searcher.PropertiesToLoad.Add("employeeID"); // User's employee ID
            searcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
            SearchResult result = searcher.FindOne();
            if (result != null)
            {
                userInfo.Name = result.Properties["displayName"][0].ToString() ?? "";
                userInfo.Email = result.Properties["mail"][0].ToString() ?? "";
                userInfo.Department = result.Properties["department"][0].ToString() ?? "";
                userInfo.Title = "";
                userInfo.PhoneNumber = "";
                userInfo.Company = result.Properties["company"][0].ToString() ?? "";
                userInfo.Country = result.Properties["co"][0].ToString() ?? "";
                //userInfo.Manager = result.Properties["manager"][0].ToString() ?? "";
                //userInfo.EmployeeID = result.Properties["employeeID"][0].ToString() ?? "";
                userInfo.Office = "";
            }
        }
        return userInfo;
    }
    public List<UserMD> GetUserMD(string userName)
    {
        string baseQuery = "SELECT * FROM MDCDB.DBO.CTUSER";
        var someCondition = true;
        SqlParameter parameter = null;
        if (someCondition)
        {
            parameter = new SqlParameter("@SomeValue", SqlDbType.VarChar)
            {
                Value = userName
            };
        }
        string query = baseQuery + (parameter != null ? " WHERE USERID = @SomeValue" : "");
        List<UserMD> data = ExecuteQueryAndMapResults<UserMD>(query, parameter != null ? new[] { parameter } : null);
        return data;
    }
    public List<T> ExecuteQueryAndMapResults<T>(string query, SqlParameter[] parameters = null) where T : new()
    {
        List<T> data = [];
        try
        {
            using SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            using SqlCommand command = new(query, connection);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                T item = new();
                foreach (var property in typeof(T).GetProperties())
                {
                    string columnName = property.Name;
                    property.SetValue(item, reader[columnName] == DBNull.Value ? null : reader[columnName]);
                }
                data.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the SQL query.");
        }
        return data;
    }
}