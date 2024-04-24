using Microsoft.Data.SqlClient;
using NETCORE.Controllers;
using System.Data;
using WebApi.Authorization;

namespace NETCORE.Classes
{
    [CustomAuthorize]
    public class Class
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<Class> _logger;

        public Class(IConfiguration configuration, ILogger<Class> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public List<T> ExecuteQueryAndMapResults<T>(string query, SqlParameter[] parameters = null) where T : new()
        {
            List<T> data = [];
            try
            {
                using SqlConnection connection = new(_connectionString);
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

        public List<T> ExecuteStoredProcedure<T>(string storedProcedureName, SqlParameter[] parameters = null) where T : new()
        {
            List<T> data = [];
            try
            {
                using SqlConnection connection = new(_connectionString);
                connection.Open();
                using SqlCommand command = new(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
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
                        int columnIndex = reader.GetOrdinal(columnName);
                        if (!reader.IsDBNull(columnIndex))
                        {
                            object columnValue = reader[columnIndex];
                            object convertedValue = ConvertToType(columnValue, property.PropertyType);
                            property.SetValue(item, convertedValue);
                        }
                        else
                        {
                            property.SetValue(item, null);
                        }
                    }
                    data.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while executing the stored procedure: {storedProcedureName}.");
            }
            return data;
        }


        private object ConvertToType(object value, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return value.ToString();
            }
            else if (targetType == typeof(DateTime) && value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss");
            }
            else
            {
                return Convert.ChangeType(value, targetType);
            }
        }
        public DataTable ExecuteSqlQuery(string query)
        {
            DataTable dataTable = new();
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
            try
            {
                using SqlConnection connection = new(connectionString);
                connection.Open();
                using SqlCommand command = new(query, connection);
                SqlDataAdapter adapter = new(command);
                adapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the SQL query.");
            }
            return dataTable;
        }
    }
}
