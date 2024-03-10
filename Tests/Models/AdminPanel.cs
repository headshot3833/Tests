using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tests.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<UserRole> Roles { get; set; } // Навигационное свойство для хранения списка ролей пользователя
    }

     public enum RoleAdmin
    {
        SuperAdmin,
        Admin,
        user,
        Role
    }
    public class UserRole
    {
        public int Id { get; set; }
        public string Role { get; set; }

        public int UserId { get; set; } // Внешний ключ для связи с пользователем
        public User User { get; set; } // Навигационное свойство для пользователя
    }

    public class CreateUserAdminViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
    public class LoginModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool keeploggedin { get; set; }
    }
}

