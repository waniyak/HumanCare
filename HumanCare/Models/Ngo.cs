﻿using System.ComponentModel.DataAnnotations;

namespace HumanCare.Models
{
    public class Ngo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }

}