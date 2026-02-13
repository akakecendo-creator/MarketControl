using MarketControl.Data;
using MarketControl.Models;
using System;
using Microsoft.Data.SqlClient;

namespace MarketControl.Services
{
    public class ProdutoService
    {
        Database db = new Database();

        public void CadastrarProduto()
        {
            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("Preço: ");
            decimal preco = decimal.Parse(Console.ReadLine());

            Console.Write("Estoque: ");
            int estoque = int.Parse(Console.ReadLine());

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO Produto VALUES (@nome, @preco, @estoque)";

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
