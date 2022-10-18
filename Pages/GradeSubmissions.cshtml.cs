using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class GradeSubmissionsModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;

        public GradeSubmissionsModel(IUserRepository userRepository, ISubmissionRepository submissionRepository, IAssignmentRepository assignmentRepository)
        {
            this.userRepository = userRepository;
            this.submissionRepository = submissionRepository;
            this.assignmentRepository = assignmentRepository;
        }

        public User User { get; set; }
        public Assignment Assignment { get; set; }
        public IEnumerable<Submission> Submissions { get; set; }

        public IActionResult OnGet(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            if(!User.IsInstructor)
            {
                return NotFound();
            }
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            Submissions = submissionRepository.GetSubmissionsByAssignment(assignmentId);

            return Page();
        }
    }
}
