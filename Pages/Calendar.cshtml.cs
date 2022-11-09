using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using JsonSerializer = System.Text.Json.JsonSerializer;
using CS3750_PlanetExpressLMS.Data;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class CalendarModel : PageModel
    {
        public readonly INotificationRepository notificationRepository;

        public CalendarModel(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        [BindProperty]
        public User user { get; set; }

        public List<Course> courses;

        public List<Notification> notifications { get; set; }

        public class CalendarEvent
        {
            public string title;
            public string startTime;
            public string endTime;
            public string startRecur;
            public string endRecur;
            public string start;
            public string end;
            public int[] daysOfWeek;
            public string display;
            public bool allDay;
            public string backgroundColor;
            public string borderColor;
            public string textColor;
        }

        public List<CalendarEvent> events = new List<CalendarEvent>();

        public string jsonEvents;

        public async Task<IActionResult> OnGet()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            notifications = notificationRepository.GetNotifications(user.ID);

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            courses = session.GetCourses();

            List<string> colors =
                new List<string>() { "#1982c4", "#ff5400", "#0ead69", "#540d6e", "#ff0054", "#277da1", "#9e0059" };
            int count = 0;

            //Iterate through courses and create a list of events
            foreach (Course course in courses)
            {
                // Create an event for class times/dates
                CalendarEvent courseEvent = new CalendarEvent();
                courseEvent.title = course.CourseName;
                courseEvent.startTime = course.StartTime.ToString("hh:mm");
                courseEvent.endTime = course.EndTime.ToString("hh:mm");
                courseEvent.startRecur = course.StartDate.ToString("yyyy-MM-dd");
                courseEvent.endRecur = course.EndDate.ToString("yyyy-MM-dd");
                courseEvent.daysOfWeek = ParseDaysOfWeek(course.Days);
                courseEvent.display = "block";
                courseEvent.allDay = false;
                courseEvent.backgroundColor = colors[count];
                events.Add(courseEvent);

                if (!user.IsInstructor)
                {
                    //Pull assignments for this course
                    IEnumerable<Assignment> assignments = session.GetAssignments();

                    //iterate through, creating calendar events for each, with the same color
                    foreach (Assignment assignment in assignments)
                    {
                        if (assignment.CourseID == course.ID)
                        {
                            CalendarEvent assignmentEvent = new CalendarEvent();
                            assignmentEvent.title = $"Due: {assignment.Name}";
                            assignmentEvent.start = assignment.CloseDateTime.ToString("yyyy-MM-dd");
                            assignmentEvent.display = "block";
                            assignmentEvent.allDay = true;
                            assignmentEvent.backgroundColor = "#ffffff";
                            assignmentEvent.borderColor = colors[count];
                            assignmentEvent.textColor = colors[count];
                            events.Add(assignmentEvent);
                        }
                    }
                }
                count++;
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
            return RedirectToPage("Calendar");
        }

    }
}
