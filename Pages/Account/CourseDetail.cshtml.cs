using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    // For now, just display course info.
    // Eventually, if the user is the instructor who created the course, allow for edits.
    public class CourseDetailModel : PageModel
    {
        private readonly ICourseRepository courseRepository;
        private readonly IUserRepository userRepository;

        public CourseDetailModel(ICourseRepository courseRepository, IUserRepository userRepository)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int userID, int courseID)
        {
            User = userRepository.GetUser(userID);
            Course = courseRepository.GetCourse(courseID);
            if (Course == null) { return NotFound(); }

            return Page();
        }
    }
}
