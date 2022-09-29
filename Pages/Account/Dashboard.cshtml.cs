using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Web.WebPages.Html;
using CS3750_PlanetExpressLMS.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class DashboardModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ICourseRepository courseRepository;
        private readonly CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext _context;

        public DashboardModel(IUserRepository userRepository, ICourseRepository courseRepository, CS3750_PlanetExpressLMSContext context)
        {
            this.courseRepository = courseRepository;
            this.userRepository = userRepository;
            this._context = context;
        }

        [BindProperty]
        public User User { get; set; }
        public IEnumerable<Course> Cards { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            Cards = courseRepository.GetUserCourses(User.ID);
            

            // Otherwise, return the page
            return Page();
        }
    }
}
