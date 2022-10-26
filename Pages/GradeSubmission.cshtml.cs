using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class GradeSubmissionModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ISubmissionRepository submissionRepository;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly INotificationRepository notificationRepository;

        public GradeSubmissionModel(IUserRepository userRepository, ISubmissionRepository submissionRepository, IAssignmentRepository assignmentRepository, INotificationRepository notificationRepository)
        {
            this.userRepository = userRepository;
            this.submissionRepository = submissionRepository;
            this.assignmentRepository = assignmentRepository;
            this.notificationRepository = notificationRepository;
        }

        public User user { get; set; }

        public Assignment Assignment { get; set; }

        public Submission Submission { get; set; }

        public string SubmissionText { get; set; }

        [BindProperty]
        public decimal Grade { get; set; }

        public string ErrorMessage { get; set; }

        public User Student { get; set; }

        public Notification notification { get; set; }

        
        public void OnGet(int submissionId)
        {
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);
            Submission = submissionRepository.GetSubmission(submissionId);
            user = session.GetUser();
            Student = userRepository.GetUser(Submission.UserID);
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


        //Download file on the click of a button
        public FileResult OnGetDownloadFile(int submissionId)
        {
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            Submission = submissionRepository.GetSubmission(submissionId);
            user = session.GetUser();
            Student = userRepository.GetUser(Submission.UserID);
            Assignment = assignmentRepository.GetAssignment(Submission.AssignmentID);
            byte[] bytes = System.IO.File.ReadAllBytes(Submission.Path);
            string ext = Path.GetExtension(Submission.Path);
            string fileName = Assignment.Name + "_" + Student.FirstName + Student.LastName + ext;

            return File(bytes, "application/octet-stream", fileName);
        }

        public IActionResult OnPost(int submissionId)
        {
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            //Set variables to "reset" page
            Submission = submissionRepository.GetSubmission(submissionId);
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            Student = userRepository.GetUser(Submission.UserID);

            Assignment = assignmentRepository.GetAssignment(Submission.AssignmentID);
            //Don't allow extra credit or negative grades
            if (this.Grade < 0 || this.Grade > Assignment.PointsPossible)
            {
                ErrorMessage = "Grade must be above zero and less than or equal to " + Assignment.PointsPossible + ".";

                //Reset text display
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

                notification = new Notification();
                notification.Title = Assignment.Name + " Graded";
                notification.UserID = Student.ID;
                notificationRepository.Add(notification);

                return Redirect("/ViewSubmissions/" + Assignment.ID);
            }
        }

    }
}
