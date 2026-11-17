using A_PRN212.Domain.Entities;

namespace A_PRN212.Application.Common
{
    /// <summary>
    /// Holds the currently signed-in user for the active application session.
    /// </summary>
    public sealed class UserSession
    {
        private static readonly Lazy<UserSession> _instance = new(() => new UserSession());

        public static UserSession Instance => _instance.Value;

        public User? CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        private UserSession()
        {
        }

        public void SetUser(User user)
        {
            CurrentUser = user;
        }

        public void Clear()
        {
            CurrentUser = null;
        }
    }
}
