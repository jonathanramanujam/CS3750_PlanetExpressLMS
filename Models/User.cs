﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CS3750_PlanetExpressLMS.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                        + "@"
                         + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("[a-zA-Z0-9]*", ErrorMessage = "Password cannot contain special characters.")]
        [StringLength(16, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        
        [MinimumAge(16, ErrorMessage = "Minimum age to use this is 16")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required]
        [Display(Name = "Select if you are student or instructor")]
        public bool IsInstructor { get; set; }

        public string Bio { get; set; }

        public byte[] Image { get; set; }
    }
}
