using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="Введите логин")]
        public string Login {  get; set; } = null!;

        [Required]
        [Display(Name = "Введите пароль")]
        public string Password { get; set; } = null!;

        public string? ReturnUrl { get; set; }
    }
}
