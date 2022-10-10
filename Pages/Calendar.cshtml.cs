using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using CS3750_PlanetExpressLMS.Data;
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
            public string startTime;
            public string endTime;
            public string startRecur;
            public string endRecur;
            public int[] daysOfWeek;
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
                newEvent.startTime = course.StartTime.ToString("hh:mm");
                newEvent.endTime = course.EndTime.ToString("hh:mm");
                newEvent.startRecur = course.StartDate.ToString("yyyy-MM-dd");
                newEvent.endRecur = course.EndDate.ToString("yyyy-MM-dd");
                newEvent.daysOfWeek = ParseDaysOfWeek(course.Days);
                events.Add(newEvent);
            }

            jsonEvents = JsonConvert.SerializeObject(events);

            // Otherwise, return the page
            return Page();
        }

        public int[] ParseDaysOfWeek(string dbDaysOfWeek)
        {
            List<int> daysOfWeek = new List<int>();
            if (dbDaysOfWeek.Contains("Sun"))
            {
                daysOfWeek.Add(0);
            }
            if (dbDaysOfWeek.Contains("Mon"))
            {
                daysOfWeek.Add(1);
            }
            if (dbDaysOfWeek.Contains("Tue"))
            {
                daysOfWeek.Add(2);
            }
            if (dbDaysOfWeek.Contains("Wed"))
            {
                daysOfWeek.Add(3);
            }
            if (dbDaysOfWeek.Contains("Thu"))
            {
                daysOfWeek.Add(4);
            }
            if (dbDaysOfWeek.Contains("Fri"))
            {
                daysOfWeek.Add(5);
            }
            if (dbDaysOfWeek.Contains("Sat"))
            {
                daysOfWeek.Add(6);
            }
            int[] daysOfWeekArr = new int[daysOfWeek.Count];
            for (int i = 0; i < daysOfWeekArr.Length; i++)
            {
                daysOfWeekArr[i] = daysOfWeek[i];
            }
            return daysOfWeekArr;
        }

    }
}
