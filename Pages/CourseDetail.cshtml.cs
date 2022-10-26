using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Pages
{
    // For now, just display course info.
    // Eventually, if the user is the instructor who created the course, allow for edits.
    public class CourseDetailModel : PageModel
    {
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly IEnrollmentRepository enrollmentRepository;

        public CourseDetailModel(IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, INotificationRepository notificationRepository, IEnrollmentRepository enrollmentRepository)
        {
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            this.notificationRepository = notificationRepository;
            this.enrollmentRepository = enrollmentRepository;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public Course course { get; set; }

        [BindProperty]
        public List<Assignment> courseAssignments { get; set; }

        [BindProperty]
        public Assignment assignment { get; set; }

        public List<Assignment> assignments;

        public List<Enrollment> enrollments;

        public Notification notification { get; set; }

        public List<Notification> notifications { get; set; }

        public List<Submission> submissions { get; set; }

        public bool[] assignmentHasSubmission { get; set; }

        public async Task<IActionResult> OnGetAsync (int courseID)
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

            // Get courses from the session
            List<Course> courses = session.GetCourses();

            foreach (Course _course in courses)
            {
                if (_course.ID == courseID)
                {
                    course = _course;
                }
            }

            if (course == null) { return NotFound(); }

            //assignment = new Assignment();

            // Try to get assignments from session
            assignments = session.GetAssignments();

            // If the user is an instructor and they will need to get assignments from the database upon reaching this page
            if (user.IsInstructor && assignments == null)
            {
                assignments = assignmentRepository.GetInstructorAssignments(user.ID).ToList();
                session.SetAssignments(assignments);
            }

            // Check for existing assignments for this course
            courseAssignments = new List<Assignment>();

            if (assignments.Count() != 0)
            {
                foreach (Assignment assignment in assignments)
                {
                    if (assignment.CourseID == courseID)
                    {
                        courseAssignments.Add(assignment);
                    }
                }
            }
            
            //If user is a student, and the course has assignments, check for submissions
            if (!user.IsInstructor && courseAssignments.Count() != 0)
            {
                // TODO: Submissions are not playing nice with the session

                //Check the session first to see if submissions have been grabbed at this point
                submissions = session.GetSubmissions();
                if (submissions == null)
                {
                    submissions = submissionRepository.GetStudentSubmissions(user.ID).ToList();
                    session.SetSubmissions(submissions);
                }

                assignmentHasSubmission = new bool[courseAssignments.Count()];

                for (int i = 0; i < courseAssignments.Count(); i++)
                {
                    foreach (Submission submission in submissions)
                    {
                        if (submission.AssignmentID == courseAssignments.ElementAt(i).ID)
                        {
                            assignmentHasSubmission[i] = true;
                            break;
                        }
                        else
                        {
                            assignmentHasSubmission[i] = false;
                        }
                    }
                }
            }
            return Page();
        }

        public IActionResult OnPost(int courseId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            assignment.CourseID = courseId;

            //Create a new assignment
            assignment = assignmentRepository.Add(assignment);

            //Update the session
            assignments = assignmentRepository.GetInstructorAssignments(user.ID).ToList();
            session.SetAssignments(assignments);

            //Update assignments in session
            session.SetAssignments(assignments);

            // Get courses from the session
            List<Course> courses = session.GetCourses();

            foreach (Course _course in courses)
            {
                if (_course.ID == courseId)
                {
                    course = _course;
                }
            }

            courseAssignments = new List<Assignment>();

            if (assignments.Count() != 0)
            {
                foreach (Assignment assignment in assignments)
                {
                    if (assignment.CourseID == courseId)
                    {
                        courseAssignments.Add(assignment);
                    }
                }
            }

            //Add notifications for students enrolled in course
            enrollments = enrollmentRepository.GetStudentsEnrolled(courseId);

            foreach (var student in enrollments)
            {
                if (student.UserID != user.ID)
                {
                    notification = new Notification();
                    notification.Title = course.Department.ToString() + " " + course.CourseNumber.ToString() + " " + assignment.Name.ToString() + " Created";
                    notification.UserID = student.UserID;
                    notificationRepository.Add(notification);
                }
            }

            return Page();
        }
    }
}
