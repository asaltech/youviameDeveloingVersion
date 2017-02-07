using System.ComponentModel.DataAnnotations;

namespace youviame.API.UserContext {
    public class RegisterExternalTokenBindingModel {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }
        [Required]
        [Display(Name = "Provider")]
        public string Provider { get; set; }
    }
}