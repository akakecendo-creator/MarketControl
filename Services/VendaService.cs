using MarketControl.Data;
using System;
using Microsoft.Data.SqlClient;

namespace MarketControl.Services
{
    public class VendaService
    {
        Database db = new Database();

        public void RegistrarVenda()
        {
            Console.Write("ID do Cliente: ");
            int clienteId = int.Parse(Console.ReadLine());

            Console.Write("ID do Produto: ");
            int produtoId = int.Parse(Console.ReadLine());

            Console.Write("Quantidade: ");
            int quantidade = int.Parse(Console.ReadLine());

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string precoSql =
                    "SELECT Preco FROM Produto WHERE Id = @id";

                SqlCommand precoCmd = new SqlCommand(precoSql, conn);
                precoCmd.Parameters.AddWithValue("@id", produtoId);

                decimal preco = (decimal)precoCmd.ExecuteScalar();
                decimal total = preco * quantidade;

                string vendaSql =
                    "INSERT INTO Venda (ClienteId, Total) " +
                    "VALUES (@cliente, @total); SELECT SCOPE_IDENTITY();";

                SqlCommand vendaCmd = new SqlCommand(vendaSql, conn);
                vendaCmd.Parameters.AddWithValue("@cliente", clienteId);
                vendaCmd.Parameters.AddWithValue("@total", total);

                int vendaId =
                    Convert.ToInt32(vendaCmd.ExecuteScalar());

                string itemSql =
                    "INSERT INTO VendaItem VALUES " +
                    "(@venda, @produto, @qtd, @preco)";

                SqlCommand itemCmd = new SqlCommand(itemSql, conn);
                itemCmd.Parameters.AddWithValue("@venda", vendaId);
                itemCmd.Parameters.AddWithValue("@produto", produtoId);
                itemCmd.Parameters.AddWithValue("@qtd", quantidade);
                itemCmd.Parameters.AddWithValue("@preco", preco);

                itemCmd.ExecuteNonQuery();
            }

            Console.WriteLine("Venda registrada com sucesso!\n");
        }
    }
}
