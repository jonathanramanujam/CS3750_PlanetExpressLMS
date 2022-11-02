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
        [StringLength(16)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        // Code snippit is good to set a range of allowed dates
        /*[Range(typeof(DateTime), "1/2/2004", "3/4/2004",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]*/
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiration Date")]
        public DateTime ExpDate { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Enter 3 digit CVV")]
        public string Cvv { get; set; }

    }
}
