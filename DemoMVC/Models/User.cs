using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Models
{
    [Index(nameof(Login), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public List<Order> Orders { get; set; } = new();

        public Role Role { get; set; } = null!;


    }
}
