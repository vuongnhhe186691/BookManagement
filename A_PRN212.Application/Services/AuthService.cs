using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly Dictionary<string, string> _pendingOtps = new(StringComparer.OrdinalIgnoreCase);

        public AuthService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public ServiceResult<User> Login(string username, string password)
        {
            string trimmedUsername = username?.Trim() ?? string.Empty;
            string trimmedPassword = password?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(trimmedUsername) || string.IsNullOrEmpty(trimmedPassword))
            {
                return ServiceResult<User>.Failure("Please enter username and password");
            }

            var user = _unitOfWork.Users.GetByUsername(trimmedUsername);
            if (user == null || !PasswordHasher.Verify(trimmedPassword, user.Password))
            {
                return ServiceResult<User>.Failure("Wrong username and password");
            }

            if (user.Role != UserRole.Admin && user.Role != UserRole.Librarian && user.Role != UserRole.Reader)
            {
                return ServiceResult<User>.Failure("Invalid user role");
            }

            // Auto-migrate legacy plaintext passwords to SHA-256 hash on successful login
            if (!PasswordHasher.IsHashed(user.Password))
            {
                user.Password = PasswordHasher.Hash(trimmedPassword);
                _unitOfWork.Users.Update(user);
                _unitOfWork.SaveChanges();
            }

            UserSession.Instance.SetUser(user);
            return ServiceResult<User>.Success(user, "Login successful");
        }

        public ServiceResult<User> Register(string fullName, string? gender, string username, string password, string email, string? phone)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(email))
            {
                return ServiceResult<User>.Failure("Please fill all required fields!");
            }

            string trimmedUsername = username.Trim();
            string trimmedEmail = email.Trim();
            string? trimmedPhone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();

            if (_unitOfWork.Users.IsUsernameExists(trimmedUsername))
            {
                return ServiceResult<User>.Failure("Username already exists!");
            }

            if (_unitOfWork.Users.IsEmailExists(trimmedEmail))
            {
                return ServiceResult<User>.Failure("Email already exists!");
            }

            if (!string.IsNullOrEmpty(trimmedPhone) && _unitOfWork.Users.IsPhoneExists(trimmedPhone))
            {
                return ServiceResult<User>.Failure("Phone already exists!");
            }

            var newUser = new User
            {
                FullName = fullName.Trim(),
                Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim(),
                Username = trimmedUsername,
                Password = PasswordHasher.Hash(password),
                Email = trimmedEmail,
                Phone = trimmedPhone,
                Role = UserRole.Reader
            };

            _unitOfWork.Users.Add(newUser);
            _unitOfWork.SaveChanges();

            return ServiceResult<User>.Success(newUser, "Sign up successfully!");
        }

        public ServiceResult SendPasswordResetOtp(string email)
        {
            string trimmedEmail = email?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(trimmedEmail))
            {
                return ServiceResult.Failure("Please enter your email.");
            }

            var user = _unitOfWork.Users.GetByEmail(trimmedEmail);
            if (user == null)
            {
                return ServiceResult.Failure("Email does not exist. Please try again.");
            }

            string otp = Random.Shared.Next(100000, 999999).ToString();
            _pendingOtps[trimmedEmail] = otp;

            try
            {
                _emailService.SendEmail(trimmedEmail, "Your OTP Code", $"Your OTP code is: {otp}");
                return ServiceResult.Success("Email sent successfully!");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error sending email: {ex.Message}");
            }
        }

        public ServiceResult<int> VerifyOtp(string email, string otp)
        {
            string trimmedEmail = email?.Trim() ?? string.Empty;
            string trimmedOtp = otp?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(trimmedOtp) ||
                !_pendingOtps.TryGetValue(trimmedEmail, out string? expectedOtp) ||
                trimmedOtp != expectedOtp)
            {
                return ServiceResult<int>.Failure("Invalid OTP. Please try again.");
            }

            var user = _unitOfWork.Users.GetByEmail(trimmedEmail);
            if (user == null)
            {
                return ServiceResult<int>.Failure("User not found!");
            }

            _pendingOtps.Remove(trimmedEmail);
            return ServiceResult<int>.Success(user.UserId, "OTP verified successfully!");
        }

        public ServiceResult ChangePassword(int userId, string newPassword, string confirmPassword)
        {
            string pass = newPassword?.Trim() ?? string.Empty;
            string confirm = confirmPassword?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(confirm))
            {
                return ServiceResult.Failure("Password and Confirm password must not be empty!");
            }

            if (pass != confirm)
            {
                return ServiceResult.Failure("Passwords do not match!");
            }

            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
            {
                return ServiceResult.Failure("User not found!");
            }

            user.Password = PasswordHasher.Hash(pass);
            _unitOfWork.Users.Update(user);
            _unitOfWork.SaveChanges();

            if (UserSession.Instance.CurrentUser?.UserId == user.UserId)
            {
                UserSession.Instance.SetUser(user);
            }

            return ServiceResult.Success("Password changed successfully!");
        }

        public bool VerifyCurrentPassword(User user, string enteredPassword)
        {
            return PasswordHasher.Verify(enteredPassword, user.Password);
        }

        public void Logout()
        {
            UserSession.Instance.Clear();
        }
    }
}
