using CS3750_PlanetExpressLMS.Data;
//using CS3750_PlanetExpressLMS.Migrations;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class SubmitAssignmentModel : PageModel
    {
        //private readonly IUserRepository userRepository;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;
        private IWebHostEnvironment _environment;


        public SubmitAssignmentModel(/*IUserRepository userRepository, */IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, IWebHostEnvironment environment)
        {
            //this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            _environment = environment;
        }

        public User user { get; set; }

        public Assignment assignment { get; set; }

        public List<Assignment> assignments { get; set; }

        public List<Submission> submissions { get; set; }

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




        public IActionResult OnGet(int userId, int assignmentId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            assignments = session.GetAssignments();
            foreach (Assignment assignment in assignments)
            {
                if (assignment.ID == assignmentId)
                {
                    this.assignment = assignment;
                }
            }

            submissions = session.GetSubmissions();
            
            if (submissions == null)
            {
                submissions = submissionRepository.GetStudentSubmissions(user.ID).ToList();
                session.SetSubmissions(submissions);
            }

            statusMessage = GetStatusMessage(userId, assignmentId);
            return Page();
        }

        public IActionResult OnPost(int userId, int assignmentId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            assignments = session.GetAssignments();
            foreach (Assignment assignment in assignments)
            {
                if (assignment.ID == assignmentId)
                {
                    this.assignment = assignment;
                }
            }

            string filePath = "";

            if (assignment.SubmissionType.Equals("FILE"))
            {
                //Add a file upload to the wwwroot/submissions folder
                filePath = FileUpload(user, assignmentId);
            }
            else //Generate a new txt file in wwwroot/submissions
            {
                filePath = TextBoxUpload(userId, assignmentId);
            }

            //Create new submission object
            submission.AssignmentID = assignmentId;
            submission.UserID = userId;
            submission.Path = filePath;
            submissionRepository.Add(submission);

            //Get submissions from database
            submissions = submissionRepository.GetStudentSubmissions(user.ID).ToList();

            //Update Submissions in session
            session.SetSubmissions(submissions);

            //Reset status message
            statusMessage = GetStatusMessage(userId, assignmentId);

            return Page();
        }

        //Add uploaded file to wwwroot folder. Return generated file path string
        public string FileUpload(User user, int assignmentId)
        {
            //Create file name and path
            var fileName = GetFileName(upload, user, assignmentId);
            var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "submissions", fileName);
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
            var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "submissions", fileName);
            //Generate file in the appropriate folder
            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(submissionString);
            writer.Close();

            return filePath;
        }

        //Append user first and last name to file name, then extension
        public string GetFileName(IFormFile Upload, User user, int assignmentId)
        {
            //user = userRepository.GetUser(userId);
            var name = Path.GetFileNameWithoutExtension(Upload.FileName);
            var ext = Path.GetExtension(Upload.FileName);
            return name + "_" + user.FirstName + user.LastName + SubmissionCopy(user, assignmentId) + ext;
        }

        //Generate a file name for a text box submission
        public string GetTextBoxFileName(User user, int assignmentId)
        {
            //user = userRepository.GetUser(user.ID);
            assignment = assignmentRepository.GetAssignment(assignmentId);
            var fileName = assignment.Name + "_" + user.FirstName + user.LastName + SubmissionCopy(user, assignmentId) + ".txt";

            return fileName;
        }

        //Check to see if the student has already submitted this assignment. If they have, return a number distinguishing this submission from the others.
        public string SubmissionCopy(User user, int assignmentId)
        {
            //Get all submissions for this assignment by this user
            var submissionsList = submissionRepository.GetSubmissionsByAssignmentUserList(assignmentId, user.ID);
            if(submissionsList.Count > 0)
            {
                return "(" + (submissionsList.Count).ToString() + ")";
            }
            else
            {
                return "";
            }    
        }

        public string GetStatusMessage(int userId, int assignmentId)
        {
            //Get all submissions for this assignment by this user
            var submissionsList = submissionRepository.GetSubmissionsByAssignmentUserList(assignmentId, userId);
            if(submissionsList.Count > 1)
            {
                return "Submitted! (" + submissionsList.Count.ToString() + ")";
            }
            else if (submissionsList.Count == 1)
            {
                return "Submitted!";
            }
            else
            {
                return "";
            }
        }

        
    }
}
