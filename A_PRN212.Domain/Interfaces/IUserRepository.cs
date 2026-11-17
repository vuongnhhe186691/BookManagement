using A_PRN212.Domain.Entities;

namespace A_PRN212.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User? GetByUsername(string username);
        User? GetByUsernameAndPassword(string username, string password);
        User? GetByEmail(string email);
        bool IsUsernameExists(string username);
        bool IsEmailExists(string email);
        bool IsPhoneExists(string phone);
        IEnumerable<User> SearchUsers(int? userId, string? fullName, string? username, string? email, string? role);
    }
}
