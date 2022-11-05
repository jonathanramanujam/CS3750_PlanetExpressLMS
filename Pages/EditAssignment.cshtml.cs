using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class EditAssignmentModel : PageModel
    {
        private readonly IAssignmentRepository assignmentRepository;

        public EditAssignmentModel(IAssignmentRepository assignmentRepository)
        {
            this.assignmentRepository = assignmentRepository;
        }

        public User user { get; set; }

        [BindProperty]
        public Assignment assignment { get; set; }

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

            //Get assignment
            assignment = assignmentRepository.GetAssignment(assignmentId);
            return Page();
        }


        public IActionResult OnPost()
        {
            assignment = assignmentRepository.Update(assignment);
            return Redirect("/CourseDetail/" + assignment.CourseID);
        }
    }
}
