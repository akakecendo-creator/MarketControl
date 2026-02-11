using System;
using MarketControl.Services;

namespace MarketControl
{
    class Program
    {
        static void Main(string[] args)
        {
            ProdutoService produto = new ProdutoService();
            ClienteService cliente = new ClienteService();
            VendaService venda = new VendaService();

            while (true)
            {
                Console.WriteLine("\n--- MARKET CONTROL ---");
                Console.WriteLine("1 - Cadastrar Produto");
                Console.WriteLine("2 - Listar Produtos");
                Console.WriteLine("3 - Cadastrar Cliente");
                Console.WriteLine("4 - Listar Clientes");
                Console.WriteLine("5 - Registrar Venda");
                Console.WriteLine("0 - Sair");

                Console.Write("Opção: ");
                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1": produto.CadastrarProduto(); break;
                    case "2": produto.ListarProdutos(); break;
                    case "3": cliente.CadastrarCliente(); break;
                    case "4": cliente.ListarClientes(); break;
                    case "5": venda.RegistrarVenda(); break;
                    case "0": return;
                    default: Console.WriteLine("Opção inválida!"); break;
                }
            }
        }
    }
}
