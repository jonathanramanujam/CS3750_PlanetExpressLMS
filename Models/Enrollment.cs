using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Enrollment
    {
        //EnrollmentID
        [Key]
        public int ID { get; set; }

        //CourseID
        [Required]
        [ForeignKey("CourseID")]
        public int CourseID { get; set; }
    }
}
