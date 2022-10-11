using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class SubmitAssignmentModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;

        public SubmitAssignmentModel(IUserRepository userRepository, IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository)
        {
            this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Assignment Assignment { get; set; }

        [BindProperty]
        public Submission Submission { get; set; }

        public IActionResult OnGet(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            return Page();
        }

        //Create a new submission entry in the database
        public IActionResult OnPost(int userId, int assignmentId, string submissionType)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            Submission.AssignmentID = assignmentId;
            Submission.UserID = userId;
            Submission.SubmissionType = submissionType;
            submissionRepository.Add(Submission);
            return Page();
        }
    }
}
