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

namespace CS3750_PlanetExpressLMS.Pages
{
    public class WelcomeModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public WelcomeModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            // Otherwise, return the page
            return Page();
        }
    }
}
