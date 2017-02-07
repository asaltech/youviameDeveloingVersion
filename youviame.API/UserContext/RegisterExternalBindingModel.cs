﻿using System.ComponentModel.DataAnnotations;

namespace youviame.API.UserContext {
    public class RegisterExternalBindingModel {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }
    }
}