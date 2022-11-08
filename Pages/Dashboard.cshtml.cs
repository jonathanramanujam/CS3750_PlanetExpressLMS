using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class DashboardModel : PageModel
    {
        public readonly ICourseRepository courseRepository;
        private readonly IAssignmentRepository assignmentRepository;

        public DashboardModel(ICourseRepository courseRepository, IAssignmentRepository assignmentRepository)
        {
            this.courseRepository = courseRepository;
            this.assignmentRepository = assignmentRepository;
        }

        [BindProperty]
        public User user { get; set; }
        public List<Course> courses { get; set; }

        public List<Assignment> assignments { get; set; }

        public List<Course> ACourse { get; set; }

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

            // Check for courses and assignments before going to the database
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
                    assignments = assignmentRepository.GetStudentAssignments(user.ID, courses).ToList();
                    session.SetAssignments(assignments);
                }
            }
            if (assignments != null)
            {
                // Get course codes for each assignment
                ACourse = new List<Course>();
                foreach (Assignment assignment in assignments)
                {
                    if (assignment != null)
                    {
                        ACourse.Add(courseRepository.GetCourse(assignment.CourseID));
                    }
                }
            }           

            return Page();
        }
    }
}
