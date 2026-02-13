using System;
using System.IO;
using Microsoft.Data.SqlClient;

namespace MarketControl.Data
{
    public class Database
    {
        private readonly string connectionString;

        public Database()
        {
            const string fileName = "appsettings.Development.json";
            string[] candidates = new[] {
                Path.Combine(Directory.GetCurrentDirectory(), fileName),
                Path.Combine(AppContext.BaseDirectory, fileName),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", fileName),
                Path.Combine(AppContext.BaseDirectory, "..", "..", fileName)
            };

            string? found = null;
            foreach (var c in candidates)
            {
                try
                {
                    var full = Path.GetFullPath(c);
                    if (File.Exists(full))
                    {
                        found = full;
                        break;
                    }
                }
                catch
                {
                    // ignorar caminhos inválidos
                }
            }

            if (found == null)
                throw new InvalidOperationException($"Arquivo '{fileName}' não encontrado. Crie-o ou renomeie o appsettings.Development.json.example com a connection string em 'ConnectionStrings:MarketControl'");

            try
            {
                using var stream = File.OpenRead(found);
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
                throw new InvalidOperationException($"Erro ao ler '{found}': {ex.Message}");
            }

            throw new InvalidOperationException($"'{fileName}' não contém 'ConnectionStrings:MarketControl'. Adicione a connection string no arquivo.");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
