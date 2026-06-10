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
        [Display(Name="Идентификатор пользователя")]
        public int Id { get; set; }
        [Display(Name = "ФИО")]
        public string Name { get; set; } = null!;
        [Display(Name = "Логин")]
        public string Login { get; set; } = null!;
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;
        [Display(Name = "Роль пользователя")]
        public int RoleId { get; set; }
        public List<Order> Orders { get; set; } = new();

        public Role Role { get; set; } = null!;


    }
}
