using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [ForeignKey("UserID")]
        [Required]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 16)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiration Date")]
        public DateTime ExpDate { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Cvv { get; set; }
    }
}
