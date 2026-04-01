using MarketControl.Data;
using System;
using Microsoft.Data.SqlClient;
using MarketControl.Security;
using MarketControl.Utils;

namespace MarketControl.Services
{
    public class VendaService
    {
        Database db = new Database();

        public void RegistrarVenda()
        {
            AccessControl.RequireOperatorOrAdministrator();

            string clienteReferencia = ConsoleInput.ReadRequiredString("Cliente (ID ou nome): ");
            string produtoReferencia = ConsoleInput.ReadRequiredString("Produto (ID ou nome): ");
            int quantidade = ConsoleInput.ReadInt("Quantidade: ", 1);

            using (var conn = db.GetConnection())
            {
                conn.Open();

                using var transaction = conn.BeginTransaction();

                try
                {
                    int clienteId = ObterClienteId(conn, transaction, clienteReferencia);
                    (int produtoId, decimal preco, int estoqueAtual) = ObterProduto(conn, transaction, produtoReferencia);

                    if (estoqueAtual < quantidade)
                    {
                        throw new InvalidOperationException("Estoque insuficiente para realizar a venda.");
                    }

                    decimal total = preco * quantidade;
                    int vendaId = InserirVenda(conn, transaction, clienteId, total);

                    InserirItemVenda(conn, transaction, vendaId, produtoId, quantidade, preco);
                    AtualizarEstoque(conn, transaction, produtoId, quantidade);

                    transaction.Commit();
                    Console.WriteLine("Venda registrada com sucesso!\n");
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }

                    Console.WriteLine($"Erro ao registrar venda: {ex.Message}\n");
                }
            }
        }

        private static int ObterClienteId(SqlConnection conn, SqlTransaction transaction, string referencia)
        {
            if (int.TryParse(referencia, out int clienteId) && clienteId > 0)
            {
                const string sqlPorId = "SELECT COUNT(1) FROM Cliente WHERE Id = @id";

                using SqlCommand cmdPorId = new SqlCommand(sqlPorId, conn, transaction);
                cmdPorId.Parameters.AddWithValue("@id", clienteId);

                if (Convert.ToInt32(cmdPorId.ExecuteScalar()) == 0)
                {
                    throw new InvalidOperationException("Cliente não encontrado.");
                }

                return clienteId;
            }

            const string sqlPorNome = "SELECT Id FROM Cliente WHERE Nome = @nome COLLATE Latin1_General_CI_AI";

            using SqlCommand cmdPorNome = new SqlCommand(sqlPorNome, conn, transaction);
            cmdPorNome.Parameters.AddWithValue("@nome", referencia);
            using SqlDataReader reader = cmdPorNome.ExecuteReader();

            if (!reader.Read())
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            int resolvedId = reader.GetInt32(reader.GetOrdinal("Id"));

            if (reader.Read())
            {
                throw new InvalidOperationException("Há mais de um cliente com esse nome. Use o ID.");
            }

            return resolvedId;
        }

        private static (int ProdutoId, decimal Preco, int Estoque) ObterProduto(SqlConnection conn, SqlTransaction transaction, string referencia)
        {
            if (int.TryParse(referencia, out int produtoId) && produtoId > 0)
            {
                const string sqlPorId = "SELECT Id, Preco, Estoque FROM Produto WHERE Id = @id";

                using SqlCommand cmdPorId = new SqlCommand(sqlPorId, conn, transaction);
                cmdPorId.Parameters.AddWithValue("@id", produtoId);
                using SqlDataReader readerPorId = cmdPorId.ExecuteReader();

                if (!readerPorId.Read())
                {
                    throw new InvalidOperationException("Produto não encontrado.");
                }

                decimal precoPorId = readerPorId.GetDecimal(readerPorId.GetOrdinal("Preco"));
                int estoquePorId = readerPorId.GetInt32(readerPorId.GetOrdinal("Estoque"));

                return (produtoId, precoPorId, estoquePorId);
            }

            const string sqlPorNome = "SELECT Id, Preco, Estoque FROM Produto WHERE Nome = @nome COLLATE Latin1_General_CI_AI";

            using SqlCommand cmdPorNome = new SqlCommand(sqlPorNome, conn, transaction);
            cmdPorNome.Parameters.AddWithValue("@nome", referencia);
            using SqlDataReader reader = cmdPorNome.ExecuteReader();

            if (!reader.Read())
            {
                throw new InvalidOperationException("Produto não encontrado.");
            }

            int resolvedId = reader.GetInt32(reader.GetOrdinal("Id"));
            decimal preco = reader.GetDecimal(reader.GetOrdinal("Preco"));
            int estoque = reader.GetInt32(reader.GetOrdinal("Estoque"));

            if (reader.Read())
            {
                throw new InvalidOperationException("Há mais de um produto com esse nome. Use o ID.");
            }

            return (resolvedId, preco, estoque);
        }

        private static int InserirVenda(SqlConnection conn, SqlTransaction transaction, int clienteId, decimal total)
        {
            const string sql = "INSERT INTO Venda (ClienteId, Total) OUTPUT INSERTED.Id VALUES (@cliente, @total);";

            using SqlCommand cmd = new SqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("@cliente", clienteId);
            cmd.Parameters.AddWithValue("@total", total);

            object? result = cmd.ExecuteScalar();

            if (result == null)
            {
                throw new InvalidOperationException("Não foi possível gerar a venda.");
            }

            return Convert.ToInt32(result);
        }

        private static void InserirItemVenda(SqlConnection conn, SqlTransaction transaction, int vendaId, int produtoId, int quantidade, decimal preco)
        {
            const string sql = "INSERT INTO VendaItem (VendaId, ProdutoId, Quantidade, PrecoUnitario) VALUES (@venda, @produto, @qtd, @preco)";

            using SqlCommand cmd = new SqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("@venda", vendaId);
            cmd.Parameters.AddWithValue("@produto", produtoId);
            cmd.Parameters.AddWithValue("@qtd", quantidade);
            cmd.Parameters.AddWithValue("@preco", preco);
            cmd.ExecuteNonQuery();
        }

        private static void AtualizarEstoque(SqlConnection conn, SqlTransaction transaction, int produtoId, int quantidade)
        {
            const string sql = "UPDATE Produto SET Estoque = Estoque - @qtd WHERE Id = @id";

            using SqlCommand cmd = new SqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("@id", produtoId);
            cmd.Parameters.AddWithValue("@qtd", quantidade);

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException("Não foi possível atualizar o estoque do produto.");
            }
        }
    }
}
