using System;

namespace MarketControl.Security
{
    public static class AccessControl
    {
        public static UserSession? CurrentUser { get; private set; }

        public static void SignIn(UserSession session)
        {
            CurrentUser = session;
        }

        public static void SignOut()
        {
            CurrentUser = null;
        }

        public static bool IsAuthenticated => CurrentUser != null;

        public static bool IsAdministrator => CurrentUser?.Role == UserRole.Administrador;

        public static void RequireAuthenticated()
        {
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Usuário não autenticado.");
            }
        }

        public static void RequireAdministrator()
        {
            RequireAuthenticated();

            if (!IsAdministrator)
            {
                throw new InvalidOperationException("Acesso negado. Apenas administradores podem executar esta ação.");
            }
        }

        public static void RequireOperatorOrAdministrator()
        {
            RequireAuthenticated();
        }
    }
}