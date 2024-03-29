﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

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
        [Display(Name = "Instructor ID")]
        public int UserID { get; set; }

        //Department
        [Required]
        [Display(Name = "Department")]
        [StringLength(40)]
        public string Department { get; set; }        

        //CourseNumber
        [Required]
        [Display(Name = "Course Number")]
        public int CourseNumber { get; set; }

        //CourseName
        [Required]
        [StringLength(100)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        //Location
        [Required]
        [StringLength(20)]
        [Display(Name = "Course Location")]
        public string CourseLocation { get; set; }

        //CreditHours
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Credit hours must be at least 1")]
        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; }

        //Days
        [Required]
        [Display(Name = "Days")]
        [StringLength(33)]
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
