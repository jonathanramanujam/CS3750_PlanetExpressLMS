using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using CS3750_PlanetExpressLMS.Data;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

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

        private ISession session;

        public async Task<IActionResult> OnGet(int id)
        {
            // Get the current session
            session = HttpContext.Session;

            // If the user does not exist in the session yet, go out to the database
            if (session.GetString("user") == null)
            {
                // Look up the user based on the id
                user = userRepository.GetUser(id);

                // If the user does not exist, return not found
                if (user == null) { return NotFound(); }

                //Serialize and store user information to the session
                session.SetString("user", JsonSerializer.Serialize(user));

                if (user.IsInstructor)
                {
                    //session.SetInt32("userIsInstructor", 1);
                    courses = courseRepository.GetInstructorCourses((int)session.GetInt32("userID"));
                    session.SetString("courses", JsonSerializer.Serialize(courses));

                }
                else
                {
                    //session.SetInt32("userIsInstructor", 0);
                    courses = courseRepository.GetStudentCourses(user.ID);
                    assignments = assignmentRepository.GetStudentAssignments(user.ID);
                    session.SetString("courses", JsonSerializer.Serialize(courses));
                    session.SetString("assignments", JsonSerializer.Serialize(assignments));
                }
            }

            // Otherwise, get values from the session
            else
            {
                user = JsonSerializer.Deserialize<User>(session.GetString("user"));
                courses = JsonSerializer.Deserialize<IEnumerable<Course>>(session.GetString("courses"));
                if (!user.IsInstructor)
                {
                    assignments = JsonSerializer.Deserialize<IEnumerable<Assignment>>(session.GetString("assignments"));
                }
            }

            return Page();
        }
    }
}
