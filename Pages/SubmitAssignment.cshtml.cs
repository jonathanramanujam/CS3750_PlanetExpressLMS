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

        public SubmitAssignmentModel(IUserRepository userRepository, IAssignmentRepository assignmentRepository)
        {
            this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Assignment Assignment { get; set; }

        public void OnGet(int userId, int assignmentId)
        {
            User = userRepository.GetUser(userId);
            Assignment = assignmentRepository.GetAssignment(assignmentId);
        }
    }
}
