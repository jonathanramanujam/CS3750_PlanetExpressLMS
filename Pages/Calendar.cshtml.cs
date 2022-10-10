using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using CS3750_PlanetExpressLMS.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ICourseRepository courseRepository;

        public CalendarModel(IUserRepository userRepository, ICourseRepository courseRepository)
        {
            this.userRepository = userRepository;
            this.courseRepository = courseRepository;
        }

        [BindProperty]
        public User User { get; set; }

        public IEnumerable<Course> courses;

        public class CalendarEvent
        {
            public string title;
            public DateTime startdate;
        }

        public List<CalendarEvent> events = new List<CalendarEvent>();

        public string jsonEvents;

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            if (User.IsInstructor)
            {
                courses = courseRepository.GetInstructorCourses(User.ID);
            }
            else
            {
                courses = courseRepository.GetStudentCourses(User.ID);
            }

            //Iterate through courses and create a list of events
            foreach (Course course in courses)
            {
                CalendarEvent newEvent = new CalendarEvent();
                newEvent.title = course.CourseName;
                newEvent.startdate = course.StartDate;
                events.Add(newEvent);
            }

            jsonEvents = JsonConvert.SerializeObject(events);

            // Otherwise, return the page
            return Page();
        }

    }
}
