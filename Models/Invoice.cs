using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [ForeignKey("UserID")]
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal FullBalance { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }
    }
}
