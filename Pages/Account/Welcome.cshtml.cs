using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class WelcomeModel : PageModel
    {
        private readonly CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext _context;

        public WelcomeModel(CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User Credential { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            Credential = await _context.User.FirstOrDefaultAsync(c => c.ID == id);

            // If the user does not exist, return not found
            if (Credential == null) { return NotFound(); }

            // Otherwise, return the page
            return Page();
        }
    }
}
