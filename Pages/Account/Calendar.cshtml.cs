using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.EntityFrameworkCore;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class CalendarModel : PageModel
    {
        private readonly CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext _context;

        public CalendarModel(CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = await _context.Users.FirstOrDefaultAsync(c => c.ID == id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            // Otherwise, return the page
            return Page();
        }

    }
}
