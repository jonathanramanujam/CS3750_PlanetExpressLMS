using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace CS3750_PlanetExpressLMS.Pages
{
    // For now, just display course info.
    // Eventually, if the user is the instructor who created the course, allow for edits.
    public class CourseDetailModel : PageModel
    {
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;
        private IWebHostEnvironment _environment;

        public CourseDetailModel(IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, IWebHostEnvironment environment)
        {
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            _environment = environment;
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

        public List<Submission> submissions { get; set; }

        public bool[] assignmentHasSubmission { get; set; }


        public async Task<IActionResult> OnGetAsync (int courseID)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

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

            // Check for existing assignments for this course
            courseAssignments = assignmentRepository.GetAssignmentsByCourse(courseID).ToList();
            
            //If user is a student, and the course has assignments, check for submissions
            if (!user.IsInstructor && courseAssignments.Count() != 0)
            {
                submissions = submissionRepository.GetSubmissionsByAssignment(courseID).ToList();

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

            // Get courses from the session
            List<Course> courses = session.GetCourses();

            foreach (Course _course in courses)
            {
                if (_course.ID == courseId)
                {
                    course = _course;
                }
            }

            courseAssignments = assignmentRepository.GetAssignmentsByCourse(course.ID).ToList();

            return Page();
        }

        public IActionResult OnPostDelete(int assignmentId, int courseId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            assignment = assignmentRepository.GetAssignment(assignmentId);

            //Get all submissions for this assignment.
            var assignmentSubmissions = submissionRepository.GetSubmissionsByAssignment(assignmentId).ToList();
            foreach (var s in assignmentSubmissions)
            {
                //Delete submission file in wwwroot.
                System.IO.File.Delete(_environment.ContentRootPath + "/" + s.Path);
                //Delete submission in database.
                submissionRepository.Delete(s.ID);
            }
            //Finally, delete the assignment.
            assignmentRepository.Delete(assignment.ID);

            return Redirect("/CourseDetail/" + courseId);
        }
    }
}
