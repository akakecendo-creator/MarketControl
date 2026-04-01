using MarketControl.Data;
using MarketControl.Security;
using System;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;

namespace MarketControl.Services
{
    public class AuthService
    {
        private readonly Database db = new Database();

        public void EnsureUserStore()
        {
            using var conn = db.GetConnection();
            conn.Open();

            const string createTableSql = @"
IF OBJECT_ID('Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE Usuario (
        Id INT IDENTITY PRIMARY KEY,
        Login VARCHAR(50) NOT NULL UNIQUE,
        SenhaHash VARCHAR(256) NOT NULL,
        SenhaSalt VARCHAR(256) NOT NULL,
        Perfil VARCHAR(20) NOT NULL,
        Ativo BIT NOT NULL DEFAULT 1,
        CONSTRAINT CK_Usuario_Perfil CHECK (Perfil IN ('Administrador', 'Operador'))
    )
END";

            using (var cmd = new SqlCommand(createTableSql, conn))
            {
                cmd.ExecuteNonQuery();
            }

            SeedDefaultUsers(conn);
        }

        public UserSession? Authenticate(string login, string password)
        {
            using var conn = db.GetConnection();
            conn.Open();

            const string sql = @"
SELECT Login, SenhaHash, SenhaSalt, Perfil
FROM Usuario
WHERE Login = @login AND Ativo = 1";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            string senhaHash = reader.GetString(reader.GetOrdinal("SenhaHash"));
            string senhaSalt = reader.GetString(reader.GetOrdinal("SenhaSalt"));

            if (!VerifyPassword(password, senhaHash, senhaSalt))
            {
                return null;
            }

            string resolvedLogin = reader.GetString(reader.GetOrdinal("Login"));
            string perfil = reader.GetString(reader.GetOrdinal("Perfil"));

            return new UserSession(resolvedLogin, ParseRole(perfil));
        }

        private void SeedDefaultUsers(SqlConnection conn)
        {
            SeedUser(conn, "admin", "admin123", UserRole.Administrador);
            SeedUser(conn, "operador", "operador123", UserRole.Operador);
        }

        private void SeedUser(SqlConnection conn, string login, string password, UserRole role)
        {
            const string existsSql = "SELECT COUNT(1) FROM Usuario WHERE Login = @login";

            using (var existsCmd = new SqlCommand(existsSql, conn))
            {
                existsCmd.Parameters.AddWithValue("@login", login);

                if (Convert.ToInt32(existsCmd.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            (string hash, string salt) = HashPassword(password);

            const string insertSql = @"
INSERT INTO Usuario (Login, SenhaHash, SenhaSalt, Perfil, Ativo)
VALUES (@login, @hash, @salt, @perfil, 1)";

            using var insertCmd = new SqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("@login", login);
            insertCmd.Parameters.AddWithValue("@hash", hash);
            insertCmd.Parameters.AddWithValue("@salt", salt);
            insertCmd.Parameters.AddWithValue("@perfil", role.ToString());
            insertCmd.ExecuteNonQuery();
        }

        private static (string Hash, string Salt) HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        private static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] salt = Convert.FromBase64String(storedSalt);
            byte[] expectedHash = Convert.FromBase64String(storedHash);
            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        private static UserRole ParseRole(string perfil)
        {
            return perfil.Equals(UserRole.Administrador.ToString(), StringComparison.OrdinalIgnoreCase)
                ? UserRole.Administrador
                : UserRole.Operador;
        }
    }
}