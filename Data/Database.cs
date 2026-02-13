using System;
using Microsoft.Data.SqlClient;

namespace MarketControl.Data
{
    public class Database
    {
        private readonly string connectionString;

        public Database()
        {
            const string file = "appsettings.Development.json";
            if (!System.IO.File.Exists(file))
                throw new InvalidOperationException($"Arquivo '{file}' não encontrado. Crie-o com a connection string em 'ConnectionStrings:MarketControl'.");

            try
            {
                using var stream = System.IO.File.OpenRead(file);
                using var doc = System.Text.Json.JsonDocument.Parse(stream);
                if (doc.RootElement.TryGetProperty("ConnectionStrings", out var cs) &&
                    cs.ValueKind == System.Text.Json.JsonValueKind.Object &&
                    cs.TryGetProperty("MarketControl", out var mc) &&
                    mc.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    var val = mc.GetString();
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        connectionString = val;
                        return;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new InvalidOperationException($"Erro ao ler '{file}': {ex.Message}");
            }

            throw new InvalidOperationException($"'{file}' não contém 'ConnectionStrings:MarketControl'. Adicione a connection string no arquivo.");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
