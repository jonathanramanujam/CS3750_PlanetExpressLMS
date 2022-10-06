using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Submission
    {
        [Key]
        public int ID { get; set; }

        //Submission type: either File Upload or Text Box
        [Required]
        [StringLength(11)]
        [Display(Name = "Submission Type")]
        public string SubmissionType { get; set; }

        [Required]
        [ForeignKey("UserID")]
        public int UserID { get; set; }

        [Required]
        [ForeignKey("AssignmentID")]
        public int AssignmentID { get; set; }

        //Need to add a member to contain the submission data later on


    }
}
