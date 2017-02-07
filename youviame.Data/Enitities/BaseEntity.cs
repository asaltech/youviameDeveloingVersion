using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace youviame.Data.Enitities {
    public class BaseEntity {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}