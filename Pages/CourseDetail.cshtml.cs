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
        private readonly ICourseRepository courseRepository;
        private readonly IUserRepository userRepository;
        private readonly IAssignmentRepository assignmentRepository;

        public CourseDetailModel(ICourseRepository courseRepository, IUserRepository userRepository, IAssignmentRepository assignmentRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this.assignmentRepository = assignmentRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        [BindProperty]
        public List<Assignment> CourseAssignments { get; set; }

        [BindProperty]
        public Assignment Assignment { get; set; }

        public async Task<IActionResult> OnGetAsync(int userID, int courseID)
        {
            User = userRepository.GetUser(userID);
            Course = courseRepository.GetCourse(courseID);
            Assignment = new Assignment();
            if (Course == null) { return NotFound(); }

            CourseAssignments = assignmentRepository.GetAssignmentsByCourse(courseID).ToList();

            return Page();
        }

        public IActionResult OnPost(int courseId)
        {
            Assignment.CourseID = courseId;
            Assignment = assignmentRepository.Add(Assignment);
            Course = courseRepository.GetCourse(courseId);
            CourseAssignments = assignmentRepository.GetAssignmentsByCourse(courseId).ToList();
            return Page();
        }
    }
}
