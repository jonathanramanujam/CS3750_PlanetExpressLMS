using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class RegisterForClassesModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IEnrollmentRepository enrollmentRepository;
        public readonly INotificationRepository notificationRepository;
        public RegisterForClassesModel(IUserRepository userRepository, ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository, INotificationRepository notificationRepository)
        {
            this.userRepository = userRepository;
            this.courseRepository = courseRepository;
            this.enrollmentRepository = enrollmentRepository;
            this.notificationRepository = notificationRepository;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public List<Course> courses { get; set; }
        [BindProperty]
        public List<Course> filteredCourses { get; set; }
        [BindProperty]
        public List<Enrollment> enrollments { get; set; }

        public List<User> instructors { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? DepCodes { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DepCode { get; set; }


        public List<Notification> notifications { get; set; }

        public IActionResult OnGet()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);
            
            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            notifications = notificationRepository.GetNotifications(user.ID);

            //Check session for ALL courses
            courses = session.GetAllCourses();
            List<string> deps = new List<string>();

            if (courses == null)
            {
                //Get all courses from the database and update session
                courses = courseRepository.GetAllCourses().ToList();

                session.SetAllCourses(courses);
            }

            //get search dropdown list
            foreach (var course in courses)
            {
                if (!deps.Contains(course.Department.ToString())) { deps.Add(course.Department.ToString()); }
            }

            DepCodes = new SelectList(deps);

            //Check session for enrollments
            enrollments = session.GetEnrollments();

            if (session.GetEnrollments() == null)
            {
                //Get a list of enrollments from the database and update session
                enrollments = enrollmentRepository.GetUserEnrollments(user.ID).ToList();

                session.SetEnrollments(enrollments);
            }
            else
            {
                enrollments = session.GetEnrollments().ToList();
            }

            instructors = userRepository.GetAllInstructors().ToList();

            return Page();
        }

        public IActionResult OnPostSearch()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);
            //DepCode = DepCodes.SelectedValue.ToString();
            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            //Reset courses so they display
            courses = session.GetAllCourses();

            //search!
            if (DepCode == null || DepCode == "All") DepCode = "";

            if (SearchString == null) SearchString = "";

            courses = courseRepository.filteredCourses(DepCode, SearchString);
            

            //Update Session

            session.SetAllCourses(courses);

            //Check session for enrollments
            enrollments = session.GetEnrollments();

            if (session.GetEnrollments() == null)
            {
                //Get a list of enrollments from the database and update session
                enrollments = enrollmentRepository.GetUserEnrollments(user.ID).ToList();

                session.SetEnrollments(enrollments);
            }
            else
            {
                enrollments = session.GetEnrollments().ToList();
            }
            
            //refill search dropdown list
            List<string> deps = new List<string>();
            foreach (var course in courseRepository.GetAllCourses().ToList())
            {
                if (!deps.Contains(course.Department.ToString())) { deps.Add(course.Department.ToString()); }
            }

            DepCodes = new SelectList(deps);



            instructors = userRepository.GetAllInstructors().ToList();

            return Page();
        }

        public IActionResult OnPostRegister(int? courseId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            //Reset courses so they display
            courses = session.GetAllCourses();

            //Create and save a new enrollment
            Enrollment en = new Enrollment();
            en.UserID = user.ID;
            en.CourseID = (int)courseId;
            en.TotalPointsPossible = 0;
            en.TotalPointsEarned = 0;
            enrollmentRepository.Add(en);
            enrollments = enrollmentRepository.GetUserEnrollments(user.ID).ToList();

            //Update Session
            session.SetEnrollments(enrollments);

            session.SetCourses(courseRepository.GetStudentCourses(user.ID).ToList());

            instructors = userRepository.GetAllInstructors().ToList();

            return Page();
        }

        public IActionResult OnPostDrop(int courseId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            courses = session.GetAllCourses();
            enrollments = session.GetEnrollments().ToList();
            //Delete enrollment based on course ID
            enrollmentRepository.Delete(enrollments.Where(en => en.CourseID == courseId).ToList()[0].ID);
            //Get enrollments from database
            enrollments = enrollmentRepository.GetUserEnrollments(user.ID).ToList();
            //Update Session
            session.SetEnrollments(enrollments);

            session.SetCourses(courseRepository.GetStudentCourses(user.ID).ToList());

            instructors = userRepository.GetAllInstructors().ToList();

            return Page();
        }

        // This method calls the database for every course available. Not sure how best to mitigate this...
        public string GetProfessorName(int userId)
        {
            foreach (User instructor in instructors)
            {
                if (instructor.ID == userId)
                {
                    return instructor.FirstName + " " + instructor.LastName;
                }
            }
            return "none";
        }

        public async Task<IActionResult> OnPostClearNotification(int id)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            notificationRepository.Delete(id);
            return RedirectToPage("RegisterForClasses");
        }
    }
}

