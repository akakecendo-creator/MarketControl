using System;
using MarketControl.Security;
using MarketControl.Services;
using MarketControl.Utils;

namespace MarketControl
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthService auth = new AuthService();
            ProdutoService produto = new ProdutoService();
            ClienteService cliente = new ClienteService();
            VendaService venda = new VendaService();

            auth.EnsureUserStore();

            while (true)
            {
                EnsureAuthenticated(auth);

                Console.WriteLine("\n--- MARKET CONTROL ---");
                Console.WriteLine($"Usuário: {AccessControl.CurrentUser!.Login} | Perfil: {AccessControl.CurrentUser.Role}");

                if (AccessControl.IsAdministrator)
                {
                    Console.WriteLine("1 - Cadastrar Produto");
                    Console.WriteLine("2 - Listar Produtos");
                    Console.WriteLine("3 - Cadastrar Cliente");
                    Console.WriteLine("4 - Listar Clientes");
                    Console.WriteLine("5 - Registrar Venda");
                }
                else
                {
                    Console.WriteLine("2 - Listar Produtos");
                    Console.WriteLine("4 - Listar Clientes");
                    Console.WriteLine("5 - Registrar Venda");
                }

                Console.WriteLine("9 - Trocar Usuário");
                Console.WriteLine("0 - Sair");

                string opcao = ConsoleInput.ReadRequiredString("Opção: ");

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            produto.CadastrarProduto();
                            break;
                        case "2":
                            produto.ListarProdutos();
                            break;
                        case "3":
                            cliente.CadastrarCliente();
                            break;
                        case "4":
                            cliente.ListarClientes();
                            break;
                        case "5":
                            venda.RegistrarVenda();
                            break;
                        case "9":
                            AccessControl.SignOut();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Opção inválida!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                }
            }
        }

        private static void EnsureAuthenticated(AuthService auth)
        {
            while (!AccessControl.IsAuthenticated)
            {
                Console.WriteLine("\n--- MARKET CONTROL ---");
                Console.WriteLine("\n--- LOGIN ---");

                string login = ConsoleInput.ReadRequiredString("Login: ");
                string senha = ConsoleInput.ReadPassword("Senha: ");

                UserSession? session = auth.Authenticate(login, senha);

                if (session == null)
                {
                    Console.WriteLine("Login ou senha inválidos.\n");
                    continue;
                }

                AccessControl.SignIn(session);
                Console.WriteLine($"Acesso liberado para {session.Login}.\n");
            }
        }
    }
}
