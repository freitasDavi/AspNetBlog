using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels;

public class RegisterViewModel : LoginViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Name { get; set; }
}