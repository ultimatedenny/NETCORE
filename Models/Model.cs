using Newtonsoft.Json;
using System.Text.Json.Serialization;
namespace NETCORE.Models
{
    public class PBI_CATEGORY
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; }

        [JsonPropertyName("CATEGORY_NAME")]
        public string CATEGORY_NAME { get; set; }
    }
    public class MENU_LIST
    {
        [JsonPropertyName("SYSTEMNAME")]
        public string SystemName { get; set; }

        [JsonPropertyName("CODE")]
        public string Code { get; set; }

        [JsonPropertyName("NAME")]
        public string Name { get; set; }

        [JsonPropertyName("PARENT")]
        public string Parent { get; set; }

        [JsonPropertyName("CONTROLLER")]
        public string Controller { get; set; }

        [JsonPropertyName("ACTION")]
        public string Action { get; set; }

        [JsonPropertyName("PARAMETER")]
        public string Parameter { get; set; }

        [JsonPropertyName("PICTURE")]
        public string Picture { get; set; }

        [JsonPropertyName("ICON")]
        public string Icon { get; set; }

        [JsonPropertyName("TARGET")]
        public string Target { get; set; }

        [JsonPropertyName("METHOD")]
        public string Method { get; set; }

        [JsonPropertyName("ADDITIONALINFO")]
        public string AdditionalInfo { get; set; }

        [JsonPropertyName("SEQUENCE")]
        public string Sequence { get; set; }

        [JsonPropertyName("STATUS")]
        public string Status { get; set; }

        [JsonPropertyName("CREATEUSER")]
        public string CreateUser { get; set; }

        [JsonPropertyName("CREATEDATE")]
        public string CreateDate { get; set; }

        [JsonPropertyName("CHANGEUSER")]
        public string ChangeUser { get; set; }

        [JsonPropertyName("CHANGEDATE")]
        public string ChangeDate { get; set; }
    }
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UserInfo
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [JsonPropertyName("Department")]
        public string Department { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("Company")]
        public string Company { get; set; }

        [JsonPropertyName("Country")]
        public string Country { get; set; }

        [JsonPropertyName("Manager")]
        public string Manager { get; set; }

        [JsonPropertyName("EmployeeID")]
        public string EmployeeID { get; set; }


        [JsonPropertyName("UserID")]
        public string UserID { get; set; }
        [JsonPropertyName("Password")]
        public string Password { get; set; }


        [JsonPropertyName("Office")]
        public string Office { get; set; }

        [JsonPropertyName("Token")]
        public string Token { get; set; }

        [JsonPropertyName("TokenExp")]
        public string TokenExp { get; set; }
    }
    public class USER_DC
    {
        [JsonProperty("samaccountname")]
        public string SamAccountName { get; set; }

        [JsonProperty("givenname")]
        public string GivenName { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("telephonenumber")]
        public string TelephoneNumber { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("userprincipalname")]
        public string UserPrincipalName { get; set; }

        [JsonProperty("lastlogontimestamp")]
        public string LastLogonTimestamp { get; set; }

        [JsonProperty("whencreated")]
        public string WhenCreated { get; set; }

        [JsonProperty("whenchanged")]
        public string WhenChanged { get; set; }
    }
    public class RESPONSE
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

    }
    public class UserMD
    {
        [JsonPropertyName("ID")]
        public int Id { get; set; }

        [JsonPropertyName("PlantCode")]
        public string PlantCode { get; set; }

        [JsonPropertyName("UserID")]
        public string UserId { get; set; }

        [JsonPropertyName("UserName")]
        public string UserName { get; set; }

        [JsonPropertyName("Password")]
        public string Password { get; set; }

        [JsonPropertyName("Department")]
        public string Department { get; set; }

        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [JsonPropertyName("Phone")]
        public string Phone { get; set; }

        [JsonPropertyName("BusinessFunction")]
        public string BusinessFunction { get; set; }

        [JsonPropertyName("Photo")]
        public string Photo { get; set; }

        [JsonPropertyName("LineID")]
        public string LineId { get; set; }

        [JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("IsWindowsAuthentication")]
        public bool IsWindowsAuthentication { get; set; }

        [JsonPropertyName("LastLoginDevice")]
        public string LastLoginDevice { get; set; }

        [JsonPropertyName("LastLoginDate")]
        public DateTime LastLoginDate { get; set; }

        [JsonPropertyName("DomainName")]
        public string DomainName { get; set; }

        [JsonPropertyName("Salutation")]
        public string Salutation { get; set; }

        [JsonPropertyName("WindowsAuth")]
        public string WindowsAuth { get; set; }

        [JsonPropertyName("EmpID")]
        public string EmpId { get; set; }

        [JsonPropertyName("CreateUser")]
        public string CreateUser { get; set; }

        [JsonPropertyName("CreateDate")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("ChangeUser")]
        public string ChangeUser { get; set; }

        [JsonPropertyName("ChangeDate")]
        public DateTime ChangeDate { get; set; }

        [JsonPropertyName("VerificationCode")]
        public string VerificationCode { get; set; }
    }
    public class CustomJsonResponse
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public List<object> Data { get; set; }
    }
    public class AppInfo
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public string LatestBuild { get; set; }
        public string Company { get; set; }
        public string Footer { get; set; }
        public string DotNetVersion { get; set; }
        public string RuntimeVersion { get; set; }
    }

}
