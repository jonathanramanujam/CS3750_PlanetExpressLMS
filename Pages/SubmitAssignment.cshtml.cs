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

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Assignment Assignment { get; set; }

        [BindProperty]
        public Submission Submission { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public IActionResult OnGet(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            return Page();
        }

        //Create a new submission entry in the database
        public IActionResult OnPost(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            Submission.AssignmentID = assignmentId;
            Submission.UserID = userId;
            
            submissionRepository.Add(Submission);
            return Page();
        }


        public IActionResult OnPostFileUpload(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            //Add a file upload to the wwwroot folder
            var fileName = GetFileName(Upload, userId);
            var file = Path.Combine(_environment.ContentRootPath, "wwwroot", "submissions", fileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                Upload.CopyTo(fileStream);
            }
            //Create new submission object
            Submission.AssignmentID = assignmentId;
            Submission.UserID = userId;
            Submission.Path = file;
            submissionRepository.Add(Submission);

            return Page();
        }

        //Append user first and last name to file name, then extension
        public string GetFileName(IFormFile Upload, int userId)
        {
            User = userRepository.GetUser(userId);
            var name = Path.GetFileNameWithoutExtension(Upload.FileName);
            var ext = Path.GetExtension(Upload.FileName);
            return name + "_" + User.FirstName + User.LastName + ext;
        }
    }
}
