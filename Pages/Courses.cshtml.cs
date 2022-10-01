using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class CoursesModel : PageModel
    {
        private readonly ICourseRepository courseRepository;
        private readonly IUserRepository userRepository;

        public CoursesModel(ICourseRepository courseRepository, IUserRepository userRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        [BindProperty]
        public List<Course> UserCourses { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Look up the user based on the id
            User = userRepository.GetUser(id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            // Look up the user courses based on the user id
            if (User.IsInstructor)
            {
                UserCourses = courseRepository.GetInstructorCourses(id);
            }
            else
            {
                UserCourses = courseRepository.GetStudentCourses(id);
            }

            // Return the page
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Look up the user based on the id
            User = userRepository.GetUser(User.ID);

            Course.UserID = User.ID;
            
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

            if (Course.Days == "none")
            {
                errorMessage = "Must choose class days";
                return Page();
            }

            // Make sure start time is before end time
            if (Course.StartTime >= Course.EndTime) 
            {
                errorMessage = "Course start time cannot be after end time";
                return Page();
            }

            if (Course.StartDate >= Course.EndDate)
            {
                errorMessage = "Course start date cannot be after end date";
                return Page();
            }

            courseRepository.Add(Course);

            return Redirect(User.ID.ToString());
        }

        public void AddWeekDay(string dayOfWeek)
        {
            if (Course.Days == "none")
            {
                Course.Days = dayOfWeek;
            }
            else
            {
                Course.Days += $", {dayOfWeek}";
            }
        }
    }
}
