namespace WebApi.Models;

using Microsoft.Extensions.Configuration.UserSecrets;
using NETCORE.Models;
using System.Text.Json.Serialization;
using WebApi.Entities;

public class AuthenticateResponse
{
    private NETCORE.Models.User user;

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public string Title { get; set; }
    public string PhoneNumber { get; set; }
    public string Company { get; set; }
    public string Country { get; set; }
    public string Manager { get; set; }
    public string EmployeeID { get; set; }
    public string Office { get; set; }
    public string Token { get; set; }

    public string UserID { get; set; }
    public string Password { get; set; }


    public AuthenticateResponse(UserInfo user, string token)
    {
        Name = user.Name;
        Email = user.Email;
        Department = user.Department;
        Title = user.Title;
        PhoneNumber = user.PhoneNumber;
        Company = user.Company;
        Country = user.Country;
        Manager = user.Manager;
        EmployeeID = user.EmployeeID;
        Office = user.Office;
        UserID = user.UserID;
        Password = user.Password;
        Token = token;
    }

}