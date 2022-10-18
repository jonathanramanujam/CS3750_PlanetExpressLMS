using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class GradeSubmissionModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ISubmissionRepository submissionRepository;

        public GradeSubmissionModel(ISubmissionRepository submissionRepository, IUserRepository userRepository)
        {
            this.submissionRepository = submissionRepository;
            this.userRepository = userRepository;
        }

        public User User { get; set; }

        public Submission Submission { get; set; }

        public void OnGet(int userId, int submissionId)
        {
            User = userRepository.GetUser(userId);
            Submission = submissionRepository.GetSubmission(submissionId);
        }
    }
}
