using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class GradeSubmissionModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ISubmissionRepository submissionRepository;
        private readonly IAssignmentRepository assignmentRepository;

        public GradeSubmissionModel(ISubmissionRepository submissionRepository, IUserRepository userRepository, IAssignmentRepository assignmentRepository)
        {
            this.submissionRepository = submissionRepository;
            this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
        }

        public User User { get; set; }

        public Assignment Assignment { get; set; }

        public Submission Submission { get; set; }

        public string SubmissionText { get; set; }

        [BindProperty]
        public decimal Grade { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet(int userId, int submissionId)
        {
            Submission = submissionRepository.GetSubmission(submissionId);
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(Submission.AssignmentID);

            if (Assignment.SubmissionType.Equals("TEXT"))
            {
                //Get string to display file text
                using (StreamReader streamReader = new StreamReader(Submission.Path, Encoding.UTF8))
                {
                    SubmissionText = streamReader.ReadToEnd();
                }
            }

        }

        public FileResult OnGetDownloadFile(int submissionId)
        {
            Submission = submissionRepository.GetSubmission(submissionId);
            User = userRepository.GetUser(Submission.UserID);
            Assignment = assignmentRepository.GetAssignment(Submission.AssignmentID);
            byte[] bytes = System.IO.File.ReadAllBytes(Submission.Path);
            string ext = Path.GetExtension(Submission.Path);
            string fileName = Assignment.Name + "_" + User.FirstName + User.LastName + ext;

            return File(bytes, "application/octet-stream", fileName);
        }

        public IActionResult OnPost(int userId, int submissionId)
        {
            Submission = submissionRepository.GetSubmission(submissionId);
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(Submission.AssignmentID);
            if (this.Grade < 0 || this.Grade > Assignment.PointsPossible)
            {
                ErrorMessage = "Grade must be above zero and less than or equal to " + Assignment.PointsPossible + ".";
                if (Assignment.SubmissionType.Equals("TEXT"))
                {
                    //Get string to display file text
                    using (StreamReader streamReader = new StreamReader(Submission.Path, Encoding.UTF8))
                    {
                        SubmissionText = streamReader.ReadToEnd();
                    } 
                }
                return Page();
            }
            else
            {
                Submission.Grade = this.Grade;
                Submission = submissionRepository.Update(Submission);
                return Redirect("/ViewSubmissions/" + userId + "/" + Assignment.ID);
            }
        }

    }
}
