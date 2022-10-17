using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CS3750_PlanetExpressLMS.Pages
{
    // For now, just display course info.
    // Eventually, if the user is the instructor who created the course, allow for edits.
    public class CourseDetailModel : PageModel
    {
        private readonly ICourseRepository courseRepository;
        private readonly IUserRepository userRepository;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;

        public CourseDetailModel(ICourseRepository courseRepository, IUserRepository userRepository, IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public Course course { get; set; }

        [BindProperty]
        public List<Assignment> courseAssignnments { get; set; }

        [BindProperty]
        public Assignment assignment { get; set; }

        public bool[] assignmentHasSubmission { get; set; }

        public async Task<IActionResult> OnGetAsync (int userID, int courseID)
        {
            // Try to get the user
            try
            {
                user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));
            }
            catch
            {
                return RedirectToPage("Login");
            }

            courses = JsonSerializer.Deserialize<IEnumerable<Course>>(HttpContext.Session.GetString("courses"));

            course = courseRepository.GetCourse(courseID);
            assignment = new Assignment();
            if (course == null) { return NotFound(); }

            courseAssignnments = assignmentRepository.GetAssignmentsByCourse(courseID).ToList();
            //Find out if each assignment has a submission or not
            assignmentHasSubmission = new bool[courseAssignnments.Count()];
            for(int i = 0; i < courseAssignnments.Count(); i++)
            {
                var assignmentSubmissions = submissionRepository.GetSubmissionsByAssignmentUserList(courseAssignnments[i].ID, userID);
                if (assignmentSubmissions.Count() != 0)
                {
                    assignmentHasSubmission[i] = true;
                }
                else
                {
                    assignmentHasSubmission[i] = false;
                }
            }

            return Page();
        }

        public IActionResult OnPost(int userID, int courseId)
        {
            user = userRepository.GetUser(userID);
            assignment.CourseID = courseId;
            assignment = assignmentRepository.Add(assignment);
            course = courseRepository.GetCourse(courseId);
            courseAssignnments = assignmentRepository.GetAssignmentsByCourse(courseId).ToList();
            return Page();
        }
    }
}
