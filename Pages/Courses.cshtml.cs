using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class CoursesModel : PageModel
    {
        private readonly ICourseRepository courseRepository;
        private readonly IUserRepository userRepository;
        public readonly INotificationRepository notificationRepository;

        public CoursesModel(ICourseRepository courseRepository, IUserRepository userRepository, INotificationRepository notificationRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.notificationRepository = notificationRepository;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public Course course { get; set; }

        [BindProperty]
        public List<Course> courses { get; set; }

        [BindProperty]
        public string errorMessage { get; set; }

        [BindProperty]
        public bool Monday { get; set; }
        [BindProperty]
        public bool Tuesday { get; set; }
        [BindProperty]
        public bool Wednesday { get; set; }
        [BindProperty]
        public bool Thursday { get; set; }
        [BindProperty]
        public bool Friday { get; set; }
        [BindProperty]
        public bool Saturday { get; set; }
        [BindProperty]
        public bool Sunday { get; set; }

        public List<Notification> notifications { get; set; }

        public async Task<IActionResult> OnGetAsync()
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

            // Get Courses from the session
            courses = session.GetCourses();

            // Return the page
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            course.UserID = user.ID;
            
            if (Monday) { AddWeekDay("Mon"); }
            if (Tuesday) { AddWeekDay("Tue"); }
            if (Wednesday) { AddWeekDay("Wed"); }
            if (Thursday) { AddWeekDay("Thu"); }
            if (Friday) { AddWeekDay("Fri"); }
            if (Saturday) { AddWeekDay("Sat"); }
            if (Sunday) { AddWeekDay("Sun"); }

            if (!ModelState.IsValid)
            {
                errorMessage = "Invalid fields";
                return Page();
            }

            if (course.Days == "none")
            {
                errorMessage = "Must choose class days";
                return Page();
            }

            // Make sure start time is before end time
            if (course.StartTime >= course.EndTime) 
            {
                errorMessage = "Course start time cannot be after end time";
                return Page();
            }

            if (course.StartDate >= course.EndDate)
            {
                errorMessage = "Course start date cannot be after end date";
                return Page();
            }

            // Add the new course
            courseRepository.Add(course);

            // update the courses in teh session
            courses = courseRepository.GetInstructorCourses(user.ID);
            session.SetCourses(courses);

            return RedirectToPage("Courses");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int courseID)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            // Get the course that the user wants to delete
            courses = session.GetCourses();
            Course courseToDelete = new Course();

            foreach (Course course in courses)
            {
                if (course.ID == courseID)
                {
                    courseToDelete = course;
                }
            }

            //Make sure the user logged in is the owner of the course
            if (courseToDelete.UserID != user.ID) 
            { 
                return RedirectToPage("Login");
            }

            // Delete the Course/Assignments/Submissions
            courseRepository.Delete(courseToDelete.ID);

            // Update course list and update session
            session.SetCourses(courseRepository.GetInstructorCourses(user.ID));

            return RedirectToPage("Courses");
        }

        public void AddWeekDay(string dayOfWeek)
        {
            if (course.Days == "none")
            {
                course.Days = dayOfWeek;
            }
            else
            {
                course.Days += $", {dayOfWeek}";
            }
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
            return RedirectToPage("Courses");
        }
    }
}
