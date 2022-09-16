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

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext _context;

        public LoginModel(CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string errorMessage { get; set; }

        [BindProperty] //Allows us to retrieve Credential values, and convert from strings to .NET types. Automates and reduces error
        public User Credential { get; set; }

        public async Task<IActionResult> OnPostAsync(int ID)
        {
            //if (!ModelState.IsValid) { return Page(); }
            
            // Get a list of credentials
            var credentials = from c in _context.User
                              select c;

            // if Email and password entries are not empty
            if (!string.IsNullOrEmpty(Credential.Email) && !string.IsNullOrEmpty(Credential.Password))
            {
                // look for Email in database
                credentials = credentials.Where(c => c.Email == Credential.Email);
                if (credentials.Count() == 0)
                {
                    errorMessage = "Email does not exist.";
                    return Page();
                }

                // then see if the password matches
                credentials = credentials.Where(c => c.Password == Credential.Password);
                if (credentials.Count() == 0)
                {
                    errorMessage = "Password does not match.";
                    return Page();
                }

                // Get the first user in the list
                Credential = credentials.First<User>();

                // proceed to welcome page
                await _context.SaveChangesAsync();
                return Redirect("Welcome/" + Credential.ID);
            }
            return Page();
        }
        
        public void OnGet()
        {

        }

    }



}
