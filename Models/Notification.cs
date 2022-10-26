using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Notification
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public int UserID { get; set; }
    }
}