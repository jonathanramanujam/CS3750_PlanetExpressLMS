using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> OnGet(int id)
        {
            // Access the current session
            // Run this for every request
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Try to get the user
            user = session.GetUser();

            // If the user does not exist in the session yet, go out to the database
            if (user == null)
            {
                // Look up the user based on the id
                user = userRepository.GetUser(id);

                // If the user does not exist, return not found
                if (user == null) { return NotFound(); }

                // Add user to session
                session.SetUser(user);

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
                    assignments = assignmentRepository.GetStudentAssignments(user.ID);
                    session.SetCourses(courses);
                    session.SetAssignments(assignments);
                }
            }

            // Otherwise, get Courses (and Assignments) from the session
            else
            {
                courses = session.GetCourses();

                if (!user.IsInstructor)
                {
                    assignments = session.GetAssignments();
                }
            }

            return Page();
        }
    }
}
