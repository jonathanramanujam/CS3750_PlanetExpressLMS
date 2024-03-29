using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class SubmitAssignmentModel : PageModel
    {
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;
        public readonly INotificationRepository notificationRepository;
        private IWebHostEnvironment _environment;


        public SubmitAssignmentModel(IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, IWebHostEnvironment environment, INotificationRepository notificationRepository)
        {
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            _environment = environment;
            this.notificationRepository = notificationRepository;
        }

        public User user { get; set; }

        public Assignment assignment { get; set; }

        public List<Assignment> assignments { get; set; }

        public List<Submission> AssignmentSubmissions { get; set; }

        //Submission links the assignment to the user along with a path to the submission file location.
        [BindProperty]
        public Submission submission { get; set; }

        //Only used if the submission is a file upload.
        [BindProperty]
        public IFormFile upload { get; set; }

        //If the submission is a text box, contains the contents of the textarea.
        [BindProperty]
        public string submissionString { get; set; }

        //Notifies the user of the latest submission.
        public string statusMessage { get; set; }

        public List<Notification> notifications { get; set; }

        public IActionResult OnGet(int assignmentId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            notifications = notificationRepository.GetNotifications(user.ID);

            assignment = assignmentRepository.GetAssignment(assignmentId);

            //Get submission (if there is one)
            var submissions = submissionRepository.GetSubmissionsByAssignmentUserList(assignmentId, user.ID);
            if(submissions.Count != 0)
            {
                submission = submissions[0];
            }
            else
            {
                submission = new Submission();
            }

            //Chart stuff
            //Get all submissions for this assignment
            AssignmentSubmissions = submissionRepository.GetSubmissionsByAssignment(assignmentId).ToList();

            statusMessage = "";
            return Page();
        }


        public IActionResult OnPost(int assignmentId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            assignment = assignmentRepository.GetAssignment(assignmentId);
            //Chart stuff
            //Get all submissions for this assignment
            AssignmentSubmissions = submissionRepository.GetSubmissionsByAssignment(assignmentId).ToList();

            //Check to see if a submission already exists. If it does, delete it.
            var submissions = submissionRepository.GetSubmissionsByAssignmentUserList(assignmentId, user.ID);
            if (submissions.Count != 0)
            {
                System.IO.File.Delete(_environment.ContentRootPath + "/" + submissions[0].Path);
                submissionRepository.Delete(submissions[0].ID);
            }

            string filePath = "";

            if (assignment.SubmissionType.Equals("FILE"))
            {
                //Add a file upload to the wwwroot/submissions folder
                filePath = FileUpload(user, assignmentId);
                if (filePath == null)
                {
                    statusMessage = "Upload cannot be empty";
                    return Page();
                }
            }
            else //Generate a new txt file in wwwroot/submissions
            {
                filePath = TextBoxUpload(user.ID, assignmentId);
            }

            //Create new submission object
            submission.AssignmentID = assignmentId;
            submission.UserID = user.ID;
            submission.Path = filePath;
            submission.SubmissionTime = System.DateTime.Now;
            submission.Grade = null;
            submissionRepository.Add(submission);

            //Reset status message
            statusMessage = "Submitted!";

            return Page();
        }

        //Add uploaded file to wwwroot folder. Return generated file path string
        public string FileUpload(User user, int assignmentId)
        {
            if (upload == null)
            {
                return null;
            }
            //Create file name and path
            var fileName = GetFileName(upload, user, assignmentId);
            var filePath = Path.Combine("wwwroot", "submissions", fileName);
            //Upload the file to the correct folder
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                upload.CopyTo(fileStream);
            }
            return filePath;
        }

        //Add .txt file to wwwroot folder containing user's text box entry. Return generated file path string
        public string TextBoxUpload(int userId, int assignmentId)
        {
            //Create file name and path
            var fileName = GetTextBoxFileName(user, assignmentId);
            var filePath = Path.Combine("wwwroot", "submissions", fileName);
            //Generate file in the appropriate folder
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(submissionString);
                    writer.Close();
                }                
            }            

            return filePath;
        }

        //Append user first and last name to file name, then extension
        public string GetFileName(IFormFile Upload, User user, int assignmentId)
        {
            //user = userRepository.GetUser(userId);
            var name = Path.GetFileNameWithoutExtension(Upload.FileName);
            var ext = Path.GetExtension(Upload.FileName);
            return name + "_" + user.FirstName + user.LastName + ext;
        }

        //Generate a file name for a text box submission
        public string GetTextBoxFileName(User user, int assignmentId)
        {
            // User = userRepository.GetUser(user.ID);
            assignment = assignmentRepository.GetAssignment(assignmentId);
            var fileName = assignment.Name + "_" + user.FirstName + user.LastName + ".txt";

            return fileName;
        }

        public async Task<IActionResult> OnPostClearNotification(int id)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            notificationRepository.Delete(id);
            return RedirectToPage("SubmitAssignment");
        }
    }
}
