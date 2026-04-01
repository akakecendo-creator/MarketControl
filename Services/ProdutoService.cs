using MarketControl.Data;
using System;
using Microsoft.Data.SqlClient;
using MarketControl.Security;
using MarketControl.Utils;

namespace MarketControl.Services
{
    public class ProdutoService
    {
        Database db = new Database();

        public void CadastrarProduto()
        {
            AccessControl.RequireAdministrator();

            string nome = ConsoleInput.ReadRequiredString("Nome: ");
            decimal preco = ConsoleInput.ReadDecimal("Preço: ", 0.01m);
            int estoque = ConsoleInput.ReadInt("Estoque: ", 0);

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO Produto (Nome, Preco, Estoque) VALUES (@nome, @preco, @estoque)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@preco", preco);
                cmd.Parameters.AddWithValue("@estoque", estoque);

                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Produto cadastrado com sucesso!\n");
        }

        public void ListarProdutos()
        {
            AccessControl.RequireOperatorOrAdministrator();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM Produto";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- PRODUTOS ---");

                while (reader.Read())
                {
                    Console.WriteLine(
                        $"{reader["Id"]} - {reader["Nome"]} | " +
                        $"Preço: {reader["Preco"]} | Estoque: {reader["Estoque"]}"
                    );
                }
            }
        }
    }
}
