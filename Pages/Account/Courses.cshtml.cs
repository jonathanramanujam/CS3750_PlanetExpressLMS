using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class CoursesModel : PageModel
    {
        private readonly CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext _context;

        public CoursesModel(CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = await _context.User.FirstOrDefaultAsync(c => c.ID == id);

            // Get a list of courses in the database
            var userCourses = from c in _context.Course
                            select c;

            // Look up the user courses based on the user id
            IQueryable<Course> courses = userCourses.Where(c => c.UserID == id);
            userCourses = courses;

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            // Otherwise, return the page
            return Page();
        }
    }
}
