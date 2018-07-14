using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_PR_01_Clinicas.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "¿Recordar este explorador?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]

        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        //[Display(Name = "¿Recordar cuenta?")]
        //public bool RememberMe { get; set; }
    }
 
  
    public class RegisterViewModel
    {
       
        public int codTipoUsuario { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 3)]
        [RegularExpression("^[A-Za-z][a-zA-Z ]+$",ErrorMessage ="Solo se admite texto con espacios")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 12)]
        [RegularExpression("^\\d+$",ErrorMessage = "Numero de identificacion debe ser numerico")]
        [Display(Name = "DPI")]
        public string Dpi { get; set; }

        [Required]
        [RegularExpression("^\\d+$", ErrorMessage = "Numero de Carnet debe ser numerico")]
        [StringLength(50, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 8)]
        [Display(Name = "Carnet")]
        public string Carnet { get; set; }
       
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public  System.DateTime FechaNacimiento { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Usuario")]
        [RegularExpression("^[A-Za-z][a-zA-Z 0-9]+$", ErrorMessage = "Se admite texto, numero")]
        public string Usuario { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
 
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }
}
