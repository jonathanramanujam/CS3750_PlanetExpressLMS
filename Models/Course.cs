using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Course
    {
        //CourseID
        [Key]
        public int ID { get; set; }

        //FK InstructorID
        [Required]
        [ForeignKey("UserID")]
        public int UserID { get; set; }

        //ClassNumber
        [Required]
        [Display(Name = "Course Number")]
        public int CourseNumber { get; set; }

        //CourseName
        [Required]
        [StringLength(20)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        //Location
        [Required]
        [StringLength(20)]
        [Display(Name = "Course Location")]
        public string CourseLocation { get; set; }

        //CreditHours
        [Required]
        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; }

        //Days
        [Required]
        public string Days { get; set; }

        //StartTime
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        //EndTime
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        //StartDate
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        //EndDate
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

    }
}
