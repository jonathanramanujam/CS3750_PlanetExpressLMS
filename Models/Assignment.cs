﻿using System;
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

        //Either FILE or TEXT
        [Required]
        [Display(Name = "Submission Type")]
        [StringLength(4)]
        public string SubmissionType { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Points cannot be negative.")]
        [Display(Name = "Points Possible")]
        public int PointsPossible { get; set; }
        [Required]
        [Display(Name = "Open Date / Time")]
        public DateTime OpenDateTime { get; set; }
        [Required]
        [Display(Name = "Close Date / Time")]
        public DateTime CloseDateTime { get; set; }

        public string Description { get; set; }
        //FK to Course
        [ForeignKey("CourseID")]
        public int CourseID { get; set; }

    }
}
