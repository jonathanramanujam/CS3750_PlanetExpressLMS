﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750_A1.Data;
using CS3750_A1.Models;

namespace CS3750_A1.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly CS3750_A1.Data.CS3750_A1Context _context;

        public RegisterModel(CS3750_A1.Data.CS3750_A1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Credential Credential { get; set; }

        [BindProperty]
        public string errorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate that the values will work for the Credential model. If not, reload the page with the validation summary. This only happens OnPost.
            if (!ModelState.IsValid) { return Page(); }

            var Email = from c in _context.Credential
                           select c;
            Email = Email.Where(c => c.Email == Credential.Email);

            // If the Email already exists, reload the register page
            if (Email.Count() != 0) 
            {
                errorMessage = $"The Email '{Email.First().Email}' already exists.";
                return Page();
            }

            // Else, add the new user credential to the database
            _context.Credential.Add(Credential);
            await _context.SaveChangesAsync();

            // Then redirect to the user's welcome page
            return Redirect("Welcome/" + Credential.ID);
        }
    }
}
