using A_PRN212.Application.Common;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;
using A_PRN212.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace A_PRN212.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(LibraryDbContext context) : base(context)
        {
        }

        public User? GetByUsername(string username)
        {
            return _dbSet.AsNoTracking()
                .FirstOrDefault(u => u.Username == username);
        }

        public User? GetByUsernameAndPassword(string username, string password)
        {
            string hashedPassword = PasswordHasher.Hash(password);
            return _dbSet.AsNoTracking()
                .FirstOrDefault(u => u.Username == username &&
                    (u.Password == hashedPassword || u.Password == password));
        }

        public User? GetByEmail(string email)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(u => u.Email == email);
        }

        public bool IsUsernameExists(string username)
        {
            return _dbSet.AsNoTracking().Any(u => u.Username == username);
        }

        public bool IsEmailExists(string email)
        {
            return _dbSet.AsNoTracking().Any(u => u.Email == email);
        }

        public bool IsPhoneExists(string phone)
        {
            return _dbSet.AsNoTracking().Any(u => u.Phone == phone);
        }

        public IEnumerable<User> SearchUsers(int? userId, string? fullName, string? username, string? email, string? role)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(u => u.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                string trimmed = fullName.Trim();
                query = query.Where(u => u.FullName.Contains(trimmed));
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                string trimmed = username.Trim();
                query = query.Where(u => u.Username.Contains(trimmed));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                string trimmed = email.Trim();
                query = query.Where(u => u.Email.Contains(trimmed));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                string trimmed = role.Trim();
                query = query.Where(u => u.Role.Contains(trimmed));
            }

            return query.ToList();
        }
    }
}
