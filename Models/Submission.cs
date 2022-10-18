using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750_PlanetExpressLMS.Models
{
    public class Submission
    {
        [Key]
        public int ID { get; set; }

        //Path to the storage location of the actual submission file
        [Required]
        public string Path { get; set; }

        [Required]
        [ForeignKey("UserID")]
        public int UserID { get; set; }

        [Required]
        [ForeignKey("AssignmentID")]
        public int AssignmentID { get; set; }

        [Required]
        [Display (Name = "Submitted")]
        public System.DateTime SubmissionTime { get; set; }

        public decimal Grade { get; set; }

    }
}
