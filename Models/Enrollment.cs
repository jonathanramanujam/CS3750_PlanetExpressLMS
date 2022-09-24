using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Enrollment
    {
        //EnrollmentID
        [Key]
        public int EnrollmentId { get; set; }

        //CourseID
        [Required]
        [ForeignKey("CourseID")]
        public int CourseID { get; set; }

        //User.ID 
        [Required]
        [ForeignKey("ID")]
        public int ID { get; set; }
    }
}
