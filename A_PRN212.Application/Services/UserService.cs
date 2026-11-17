using A_PRN212.Application.Common;
using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Enums;
using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _unitOfWork.Users.GetAll().ToList();
        }

        public IEnumerable<User> SearchUsers(int? userId, string? fullName, string? username, string? email, string? role)
        {
            return _unitOfWork.Users.SearchUsers(userId, fullName, username, email, role).ToList();
        }

        public ServiceResult AddUser(string fullName, string? gender, string username, string? password, string email, string? phone, string? role)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                return ServiceResult.Failure("FullName, Username and Email are required.");
            }

            string trimmedUsername = username.Trim();
            string trimmedEmail = email.Trim();
            string? trimmedPhone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();

            if (_unitOfWork.Users.IsUsernameExists(trimmedUsername))
            {
                return ServiceResult.Failure("Username already exists!");
            }

            if (_unitOfWork.Users.IsEmailExists(trimmedEmail))
            {
                return ServiceResult.Failure("Email already exists!");
            }

            if (!string.IsNullOrEmpty(trimmedPhone) && _unitOfWork.Users.IsPhoneExists(trimmedPhone))
            {
                return ServiceResult.Failure("Phone already exists!");
            }

            var user = new User
            {
                FullName = fullName.Trim(),
                Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim(),
                Username = trimmedUsername,
                Password = PasswordHasher.EnsureHashed(string.IsNullOrWhiteSpace(password) ? "123456" : password.Trim()),
                Email = trimmedEmail,
                Phone = trimmedPhone,
                Role = string.IsNullOrWhiteSpace(role) ? UserRole.Reader : role.Trim()
            };

            _unitOfWork.Users.Add(user);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("User added successfully!");
        }

        public ServiceResult UpdateUser(int userId, string fullName, string? gender, string username, string? password, string email, string? phone, string? role)
        {
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
            {
                return ServiceResult.Failure("User not found!");
            }

            user.FullName = fullName.Trim();
            user.Gender = string.IsNullOrWhiteSpace(gender) ? null : gender.Trim();
            user.Username = username.Trim();
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.Password = PasswordHasher.EnsureHashed(password.Trim());
            }
            user.Email = email.Trim();
            user.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            if (!string.IsNullOrWhiteSpace(role))
            {
                user.Role = role.Trim();
            }

            _unitOfWork.Users.Update(user);
            _unitOfWork.SaveChanges();

            if (UserSession.Instance.CurrentUser?.UserId == user.UserId)
            {
                UserSession.Instance.SetUser(user);
            }

            return ServiceResult.Success("User updated successfully!");
        }

        public ServiceResult<User> UpdateProfile(int userId, string fullName, string? gender, string username, string password, string email, string? phone)
        {
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
            {
                return ServiceResult<User>.Failure("User not found!");
            }

            user.FullName = fullName.Trim();
            user.Gender = string.IsNullOrWhiteSpace(gender) ? user.Gender : gender.Trim();
            user.Username = username.Trim();
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.Password = PasswordHasher.EnsureHashed(password);
            }
            user.Email = email.Trim();
            user.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();

            _unitOfWork.Users.Update(user);
            _unitOfWork.SaveChanges();

            UserSession.Instance.SetUser(user);
            return ServiceResult<User>.Success(user, "Change information successfully");
        }

        public ServiceResult DeleteUser(int userId)
        {
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
            {
                return ServiceResult.Failure("User not found!");
            }

            _unitOfWork.Users.Delete(user);
            _unitOfWork.SaveChanges();
            return ServiceResult.Success("User deleted successfully!");
        }

        public int EnsureAllPasswordsHashed()
        {
            var users = _unitOfWork.Users.GetAll().ToList();
            int updatedCount = 0;

            foreach (var user in users)
            {
                if (!PasswordHasher.IsHashed(user.Password))
                {
                    user.Password = PasswordHasher.Hash(user.Password ?? string.Empty);
                    _unitOfWork.Users.Update(user);
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                _unitOfWork.SaveChanges();
            }

            return updatedCount;
        }
    }
}
