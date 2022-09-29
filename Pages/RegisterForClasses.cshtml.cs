using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CS3750_A1.Pages
{
    public class RegisterForClassesModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IEnrollmentRepository enrollmentRepository;

        public RegisterForClassesModel(IUserRepository userRepository, ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
        {
            this.userRepository = userRepository;
            this.courseRepository = courseRepository;
            this.enrollmentRepository = enrollmentRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public List<Course> Courses { get; set; }

        [BindProperty]
        public List<int> EnrolledCourses { get; set; }

        public IActionResult OnGet(int? id)
        {
            EnrolledCourses = new List<int>();

            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }
            //Get a list of all courses
            Courses = courseRepository.GetAllCourses().ToList();

            //Get a list of enrollments by user
            var Enrollments = enrollmentRepository.GetUserEnrollments(User.ID).ToList();
            //Convert that to a simple list of course IDs
            foreach(var e in Enrollments)
            {
                EnrolledCourses.Add(e.CourseID);
            }

            return Page();
        }

        public IActionResult OnPostRegister(int? userId, int? courseId)
        {
            //Reset courses so they display
            Courses = courseRepository.GetAllCourses().ToList();
            //Create and save a new enrollment
            Enrollment en = new Enrollment();
            en.UserID = (int)userId;
            en.CourseID = (int)courseId;
            enrollmentRepository.Add(en);

            return Page();
        }

        public string GetProfessorName(int userId)
        {
            User prof = userRepository.GetUser(userId);
            return prof.FirstName + " " + prof.LastName;
        }
    }
}

