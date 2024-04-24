using Microsoft.AspNetCore.Mvc;
using NETCORE.Models;
using WebApi.Authorization;
using NETCORE.Classes;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;
using System.DirectoryServices;
using System.Globalization;

namespace NETCORE.Controllers
{
    [CustomAuthorize]
    public class MasterDataController(ILogger<MasterDataController> logger, IConfiguration configuration, Class dataClass, CookieHelper cookieHelper, IJwtUtils jwtUtils) : Controller
    {
        private readonly ILogger<MasterDataController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly Class _dataClass = dataClass;
        private readonly CookieHelper _cookieHelper = cookieHelper;
        private readonly IJwtUtils _jwtUtils = jwtUtils;
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult GET_MENU_LIST()
        {
            try
            {
                string storedProcedureName = "GET_MENU_LIST";
                var Condition = true;
                SqlParameter? parameter = null;
                if (Condition)
                {
                    var token = _cookieHelper.GetCookies("TOKEN");
                    var userInfo = _jwtUtils.ValidateJwtToken(token);
                    parameter = new SqlParameter("@USERID", SqlDbType.VarChar)
                    {
                        Value = userInfo.EmployeeID ?? ""
                    };
                }
                List<MENU_LIST> data = _dataClass.ExecuteStoredProcedure<MENU_LIST>(storedProcedureName, parameter != null ? [parameter] : null);
                return Ok(data);
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.ActionDescriptor.ActionName;
                _logger.LogError(ex, $"An error occurred in while processing the {actionName} data.");
                return StatusCode(500, new { error = $"Internal server error in {actionName}. Please try again later or contact support." });
            }
        }
        public IActionResult GET_MENU_LIST_2()
        {
            try
            {
                string baseQuery = "SELECT * FROM PBI_CATEGORY";
                var someCondition = true;
                SqlParameter parameter = null;

                if (someCondition)
                {
                    parameter = new SqlParameter("@SomeValue", SqlDbType.VarChar)
                    {
                        Value = "SomeSecureValue"
                    };
                }
                string query = baseQuery + (parameter != null ? " WHERE SomeColumn = @SomeValue" : "");

                List<PBI_CATEGORY> data = _dataClass.ExecuteQueryAndMapResults<PBI_CATEGORY>(query, parameter != null ? new[] { parameter } : null);
                return Ok(data);
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.ActionDescriptor.ActionName;
                _logger.LogError(ex, $"An error occurred in while processing the {actionName} data.");
                return StatusCode(500, new { error = $"Internal server error in {actionName}. Please try again later or contact support." });
            }
        }
        public IActionResult GET_USER_DC()
        {
            try
            {
                var DELETE = DELETE_USER_DC();
                if (DELETE != null)
                {
                    SCAN_SC();
                }
                string storedProcedureName = "GET_USER_DC";
                var Condition = false;
                SqlParameter? parameter = null;
                if (Condition)
                {
                    var token = _cookieHelper.GetCookies("TOKEN");
                    var userInfo = _jwtUtils.ValidateJwtToken(token);
                    parameter = new SqlParameter("@USERID", SqlDbType.VarChar)
                    {
                        Value = userInfo.EmployeeID ?? ""
                    };
                }
                List<USER_DC> data = _dataClass.ExecuteStoredProcedure<USER_DC>(storedProcedureName, parameter != null ? [parameter] : null);
                return Ok(new { data });
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.ActionDescriptor.ActionName;
                _logger.LogError(ex, $"An error occurred in while processing the {actionName} data.");
                return StatusCode(500, new { error = $"Internal server error in {actionName}. Please try again later or contact support." });
            }
        }
        public void SCAN_SC()
        {
            DataTable tblPBIUserDC = new("USER_DC");
            tblPBIUserDC.Columns.AddRange(
            [
                new DataColumn("SamAccountName", typeof(string)),
                new DataColumn("GivenName", typeof(string)),
                new DataColumn("Surname", typeof(string)),
                new DataColumn("Mail", typeof(string)),
                new DataColumn("TelephoneNumber", typeof(string)),
                new DataColumn("Department", typeof(string)),
                new DataColumn("Company", typeof(string)),
                new DataColumn("Country", typeof(string)),
                new DataColumn("UserPrincipalName", typeof(string)),
                new DataColumn("LastLogonTimestamp", typeof(DateTime)),
                new DataColumn("WhenCreated", typeof(DateTime)),
                new DataColumn("WhenChanged", typeof(DateTime))
             ]);


            var serviceProvider = HttpContext.RequestServices;
            var _cookieHelper = serviceProvider.GetRequiredService<CookieHelper>();
            var _jwtUtils = serviceProvider.GetRequiredService<IJwtUtils>();
            var token = _cookieHelper.GetCookies("TOKEN");
            var userInfo = _jwtUtils.ValidateJwtToken(token);

            using (DirectoryEntry entry = new("LDAP://" + "SHIMANOACE", (userInfo.UserID ?? ""), (userInfo.Password ?? "")))
            {
                using DirectorySearcher searcher = new(entry);
                searcher.Filter = "(objectClass=user)";
                searcher.PropertiesToLoad.AddRange(new string[] {
                            "sAMAccountName", "givenName", "sn", "mail", "telephoneNumber",
                            "department", "company", "co", "userPrincipalName",
                            "lastLogonTimestamp", "whenCreated", "whenChanged"
                        });
                searcher.PageSize = 1000;
                searcher.SizeLimit = 0;
                int totalCount = 0;
                SearchResultCollection results;
                do
                {
                    results = searcher.FindAll();
                    foreach (SearchResult result in results)
                    {
                        var row = tblPBIUserDC.NewRow();
                        row["sAMAccountName"] = GetStringProperty(result, "sAMAccountName");
                        row["givenName"] = GetStringProperty(result, "givenName");
                        row["surname"] = GetStringProperty(result, "sn");
                        row["mail"] = GetStringProperty(result, "mail");
                        row["telephoneNumber"] = GetStringProperty(result, "telephoneNumber");
                        row["department"] = GetStringProperty(result, "department");
                        row["company"] = GetStringProperty(result, "company");
                        row["country"] = GetStringProperty(result, "co");
                        row["userPrincipalName"] = GetStringProperty(result, "userPrincipalName");
                        row["lastLogonTimestamp"] = GetDateTimeProperty(result, "lastLogonTimestamp");
                        row["whenCreated"] = GetStringProperty(result, "whenCreated");
                        row["whenChanged"] = GetStringProperty(result, "whenChanged");

                        tblPBIUserDC.Rows.Add(row);
                        totalCount++;
                    }
                    searcher.FindAll().Dispose();
                } while (results.Count == searcher.PageSize);
            }
            string tableName = "USER_DC";
            string connectionString = _connectionString;
            BulkInsert(tblPBIUserDC, connectionString, tableName);
        }
        static void BulkInsert(DataTable dataTable, string connectionString, string tableName)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();
            using SqlBulkCopy bulkCopy = new(connection);
            bulkCopy.DestinationTableName = tableName;
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }
            bulkCopy.BulkCopyTimeout = 60;
            bulkCopy.BatchSize = 1000;
            bulkCopy.WriteToServer(dataTable);
        }
        private static string GetStringProperty(SearchResult result, string propertyName)
        {
            if (result.Properties.Contains(propertyName) && result.Properties[propertyName].Count > 0)
            {
                return result.Properties[propertyName][0].ToString();
            }
            return string.Empty;
        }
        private static object GetDateTimeProperty(SearchResult result, string propertyName)
        {
            if (result.Properties.Contains(propertyName) && result.Properties[propertyName].Count > 0)
            {
                string dateString = result.Properties[propertyName][0].ToString();
                if (DateTime.TryParseExact(dateString, "yyyyMMddHHmmss.0Z", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDateTime))
                {
                    return parsedDateTime;
                }
                else
                {
                    string fileTimeString = dateString;
                    long fileTime;
                    if (long.TryParse(fileTimeString, out fileTime))
                    {
                        DateTime utcDateTime = DateTime.FromFileTime(fileTime);
                        DateTime localDateTime = utcDateTime.ToLocalTime();
                        return localDateTime;
                    }
                }
            }
            return DBNull.Value;
        }
        public IActionResult DELETE_USER_DC()
        {
            try
            {
                string storedProcedureName = "DELETE_USER_DC";
                var Condition = false;
                SqlParameter? parameter = null;
                if (Condition)
                {
                    var token = _cookieHelper.GetCookies("TOKEN");
                    var userInfo = _jwtUtils.ValidateJwtToken(token);
                    parameter = new SqlParameter("@USERID", SqlDbType.VarChar)
                    {
                        Value = userInfo.EmployeeID ?? ""
                    };
                }
                List<RESPONSE> data = _dataClass.ExecuteStoredProcedure<RESPONSE>(storedProcedureName, parameter != null ? [parameter] : null);
                return Ok(data);
            }
            catch (Exception ex)
            {
                string actionName = ControllerContext.ActionDescriptor.ActionName;
                _logger.LogError(ex, $"An error occurred in while processing the {actionName} data.");
                return StatusCode(500, new { error = $"Internal server error in {actionName}. Please try again later or contact support." });
            }
        }
    }
}
