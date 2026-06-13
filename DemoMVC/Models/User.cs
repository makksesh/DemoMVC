using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    [Index(nameof(Login), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [MaxLength(50)]
        public string Login { get; set; } = null!;
        [MaxLength(50)]
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }

        public List<Order> Orders { get; set; } = new();
        public Role Role { get; set; } = null!;


    }
}
