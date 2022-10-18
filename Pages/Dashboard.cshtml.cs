using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using CS3750_PlanetExpressLMS.Data;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IAssignmentRepository assignmentRepository;

        public DashboardModel(IUserRepository userRepository, ICourseRepository courseRepository, IAssignmentRepository assignmentRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
        }

        [BindProperty]
        public User user { get; set; }
        public IEnumerable<Course> courses { get; set; }

        public IEnumerable<Assignment> assignments { get; set; }

        public async Task<IActionResult> OnGet()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            courses = session.GetCourses();
            assignments = session.GetAssignments();

            if (courses == null)
            {
                // If the user is an instructor
                if (user.IsInstructor)
                {
                    // Get courses from database, then store in session
                    courses = courseRepository.GetInstructorCourses(user.ID);
                    session.SetCourses(courses);
                }
                else
                {
                    // Get courses and Assignments from database, then store in session
                    courses = courseRepository.GetStudentCourses(user.ID);
                    session.SetCourses(courses);
                    assignments = assignmentRepository.GetStudentAssignments(user.ID);
                    session.SetAssignments(assignments);
                }
            }

            return Page();
        }
    }
}
