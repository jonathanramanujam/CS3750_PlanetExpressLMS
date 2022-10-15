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
        public User User { get; set; }
        public IEnumerable<Course> Cards { get; set; }

        public IEnumerable<Assignment> todoList { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            //Add user information to the session
            HttpContext.Session.SetInt32("userId", User.ID);
            HttpContext.Session.SetString("userFirstName", User.FirstName);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            if (User.IsInstructor)
            {
                Cards = courseRepository.GetInstructorCourses(User.ID);
            }
            else
            {
                Cards = courseRepository.GetStudentCourses(User.ID);
                todoList = assignmentRepository.GetStudentAssignments(User.ID);
            }

            // Otherwise, return the page
            return Page();
        }
    }
}
