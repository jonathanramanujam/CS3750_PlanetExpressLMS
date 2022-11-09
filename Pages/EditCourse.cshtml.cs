using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class EditCourseModel : PageModel
    {
        private readonly ICourseRepository courseRepository;

        public EditCourseModel(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public Course course { get; set; }

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

        public async Task<IActionResult> OnGet(int courseID)
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

            foreach (Course course in courses)
            {
                if (course.ID == courseID)
                {
                    this.course = course;
                }
            }

            ParseDates(course);

            return Page();

        }

        public async Task<IActionResult> OnPostEdit()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            course.Days = "none";

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

            // Update Course and session
            courseRepository.Update(course);

            session.SetCourses(courseRepository.GetInstructorCourses(user.ID));

            return RedirectToPage("Courses");
        }

        public void ParseDates(Course course)
        {
            if (course.Days.Contains("Mon"))
            {
                Monday = true;
            }
            if (course.Days.Contains("Tue"))
            {
                Tuesday = true;
            }
            if (course.Days.Contains("Wed"))
            {
                Wednesday = true;
            }
            if (course.Days.Contains("Thu"))
            {
                Thursday = true;
            }
            if (course.Days.Contains("Fri"))
            {
                Friday = true;
            }
            if (course.Days.Contains("Sat"))
            {
                Saturday = true;
            }
            if (course.Days.Contains("Sun"))
            {
                Sunday = true;
            }
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
    }
}
