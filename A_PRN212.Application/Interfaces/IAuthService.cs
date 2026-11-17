using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;

namespace A_PRN212.Application.Interfaces
{
    public interface IAuthService
    {
        ServiceResult<User> Login(string username, string password);
        ServiceResult<User> Register(string fullName, string? gender, string username, string password, string email, string? phone);
        ServiceResult SendPasswordResetOtp(string email);
        ServiceResult<int> VerifyOtp(string email, string otp);
        ServiceResult ChangePassword(int userId, string newPassword, string confirmPassword);
        bool VerifyCurrentPassword(User user, string enteredPassword);
        void Logout();
    }
}
