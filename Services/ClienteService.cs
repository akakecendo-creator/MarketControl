using MarketControl.Data;
using System;
using System.Data.SqlClient;

namespace MarketControl.Services
{
    public class ClienteService
    {
        Database db = new Database();

        public void CadastrarCliente()
        {
            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO Cliente VALUES (@nome, @email)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);

                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Cliente cadastrado!\n");
        }

        public void ListarClientes()
        {
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
