using Shaiya.Origin.Common.Logging;
using System;
using System.Data.SqlClient;

namespace Shaiya.Origin.Database.Connector
{
    public class DatabaseConnector
    {
        public bool Connect()
        {
            var connection = GetConnection();

            // Check the connection is valid
            try
            {
                using (connection)
                {
                    connection.Open();
                    connection.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return false;
            }
        }

        public SqlConnection GetConnection(string database = "")
        {
            // Create a new connection instance
            var connection = new SqlConnection();

            // The connection string
            string connString = $"Data Source=127.0.0.1;User ID=Shaiya;Password=Shaiya123;Database={database};";

            // Set the connection string
            connection.ConnectionString = connString;

            // Connect to the database
            return connection;
        }
    }
}