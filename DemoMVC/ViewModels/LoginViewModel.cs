using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string Login {  get; set; } = null!;

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
