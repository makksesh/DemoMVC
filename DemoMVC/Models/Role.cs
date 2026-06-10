using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    /// <summary>
    /// Роль пользователя для <see cref="Models.User"/>
    /// </summary>
    public class Role
    {
        [Display(Name = "Идентификатор роли")]
        public int Id { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; } = null!;
        public List<User> Users { get; set; } = new();
    }
}
