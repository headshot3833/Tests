using MvcApp.Models;
using Tests.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Tests.Services
{
    public class AuthService
    {
        private readonly Aplicationdb _context;

        public AuthService(Aplicationdb context, IConfiguration configuration)
        {
            _context = context;
        }
        public async Task<User> GetUserByLoginAsync(string login, string role)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Login == login && u.Roles.Any(r => r.Role == role));
        }
        public bool VerifyPassword(User user, string password)
        {
            if (user == null)
                return false;

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return user.Password == sb.ToString();
            }
        }
    }
}