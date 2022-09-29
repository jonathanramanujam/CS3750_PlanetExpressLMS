using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CS3750_A1.Pages
{
    public class RegisterForClassesModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ICourseRepository courseRepository;

        public RegisterForClassesModel(IUserRepository userRepository, ICourseRepository courseRepository)
        {
            this.userRepository = userRepository;
            this.courseRepository = courseRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public List<Course> Courses { get; set; }

        public IActionResult OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }
            //Get a list of all courses
            Courses = courseRepository.GetAllCourses().ToList();

            // Otherwise, return the page
            return Page();
        }

        public string GetProfessorName(int userId)
        {
            User prof = userRepository.GetUser(userId);
            return prof.FirstName + " " + prof.LastName;
        }
    }
}

