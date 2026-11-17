using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;

namespace A_PRN212.Application.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> SearchUsers(int? userId, string? fullName, string? username, string? email, string? role);
        ServiceResult AddUser(string fullName, string? gender, string username, string? password, string email, string? phone, string? role);
        ServiceResult UpdateUser(int userId, string fullName, string? gender, string username, string? password, string email, string? phone, string? role);
        ServiceResult<User> UpdateProfile(int userId, string fullName, string? gender, string username, string password, string email, string? phone);
        ServiceResult DeleteUser(int userId);
        int EnsureAllPasswordsHashed();
    }
}
