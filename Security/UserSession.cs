namespace MarketControl.Security
{
    public class UserSession
    {
        public UserSession(string login, UserRole role)
        {
            Login = login;
            Role = role;
        }

        public string Login { get; }

        public UserRole Role { get; }
    }
}