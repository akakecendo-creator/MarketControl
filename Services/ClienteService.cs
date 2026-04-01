using MarketControl.Data;
using System;
using Microsoft.Data.SqlClient;
using MarketControl.Security;
using MarketControl.Utils;

namespace MarketControl.Services
{
    public class ClienteService
    {
        Database db = new Database();

        public void CadastrarCliente()
        {
            AccessControl.RequireAdministrator();

            string nome = ConsoleInput.ReadRequiredString("Nome: ");
            string email = ConsoleInput.ReadRequiredString("Email: ");

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO Cliente (Nome, Email) VALUES (@nome, @email)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);

                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Cliente cadastrado!\n");
        }

        public void ListarClientes()
        {
            AccessControl.RequireOperatorOrAdministrator();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM Cliente";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- CLIENTES ---");

                while (reader.Read())
                {
                    Console.WriteLine(
                        $"{reader["Id"]} - {reader["Nome"]} | {reader["Email"]}"
                    );
                }
            }
        }
    }
}
