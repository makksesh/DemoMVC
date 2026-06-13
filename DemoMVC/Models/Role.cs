using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Роль пользователя для <see cref="Models.User"/>
    /// </summary>
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public List<User> Users { get; set; } = new();
    }
}
