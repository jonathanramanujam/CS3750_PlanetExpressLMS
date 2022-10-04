using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Assignment
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Assignment Name")]
        public string Name { get; set; }

        //Either File Upload or Text Box
        [Required]
        [Display(Name = "Submission Type")]
        [StringLength(11)]
        public string SubmissionType { get; set; }
        [Required]
        [Display(Name = "Points Possible")]
        public int PointsPossible { get; set; }
        [Required]
        [Display(Name = "Open Date / Time")]
        public DateTime OpenDateTime { get; set; }
        [Required]
        [Display(Name = "Close Date / Time")]
        public DateTime CloseDateTime { get; set; }
        //FK to Course
        [ForeignKey("CourseID")]
        public int CourseID { get; set; }

    }
}
