using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class SubmitAssignmentModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;
        private IWebHostEnvironment _environment;

        public SubmitAssignmentModel(IUserRepository userRepository, IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, IWebHostEnvironment environment)
        {
            this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            _environment = environment;
        }

        public User User { get; set; }

        public Assignment Assignment { get; set; }

        [BindProperty]
        public Submission Submission { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        [BindProperty]
        public string SubmissionString { get; set; }

        public IActionResult OnGet(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            return Page();
        }

        public IActionResult OnPost(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            string filePath = "";

            if (Assignment.SubmissionType.Equals("FILE"))
            {
                //Add a file upload to the wwwroot/submissions folder
                filePath = FileUpload(userId);
            }
            else //Generate a new txt file in wwwroot/submissions
            {
                filePath = TextBoxUpload(userId, assignmentId);
            }

            //Create new submission object
            Submission.AssignmentID = assignmentId;
            Submission.UserID = userId;
            Submission.Path = filePath;
            submissionRepository.Add(Submission);

            return Page();
        }

        public string FileUpload(int userId)
        {
            var fileName = GetFileName(Upload, userId);
            var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "submissions", fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                Upload.CopyTo(fileStream);
            }
            return filePath;
        }

        public string TextBoxUpload(int userId, int assignmentId)
        {
            //Create file name and path
            var fileName = GetTextBoxFileName(userId, assignmentId);
            var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "submissions", fileName);
            //Generate file in the appropriate folder
            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(SubmissionString);
            writer.Close();

            return filePath;
        }

        //Append user first and last name to file name, then extension
        public string GetFileName(IFormFile Upload, int userId)
        {
            User = userRepository.GetUser(userId);
            var name = Path.GetFileNameWithoutExtension(Upload.FileName);
            var ext = Path.GetExtension(Upload.FileName);
            return name + "_" + User.FirstName + User.LastName + ext;
        }

        //Generate a file name for a text box submission
        public string GetTextBoxFileName(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            return Assignment.Name + "_" + User.FirstName + User.LastName + ".txt";
        }
    }
}
