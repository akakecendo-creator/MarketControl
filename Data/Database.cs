using System.Data.SqlClient;

namespace MarketControl.Data
{
    public class Database
    {
        private string connectionString =
            "Server=localhost;Database=MarketControl;Trusted_Connection=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
