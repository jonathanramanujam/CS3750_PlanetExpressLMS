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
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

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

        [BindProperty] //Allows us to retrieve User values, and convert from strings to .NET types. Automates and reduces error
        public User User { get; set; }

        public async Task<IActionResult> OnPostAsync(int ID)
        {
            //if (!ModelState.IsValid) { return Page(); }
            
            // Get a list of users
            var users = from c in _context.User
                              select c;

            // if Email and password entries are not empty
            if (!string.IsNullOrEmpty(User.Email) && !string.IsNullOrEmpty(User.Password))
            {
                // look for Email in database
                users = users.Where(c => c.Email == User.Email);
                if (users.Count() == 0)
                {
                    errorMessage = "Email does not exist.";
                    return Page();
                }

                // then see if the password matches
                //users = users.Where(c => c.Password == User.Password);
                //if (users.Count() == 0)
                //{
                //    errorMessage = "Password does not match.";
                //    return Page();
                //}
                // If the password does not match, return the page with an error
                if (!VerifyHashedPassword(users.First<User>().Password, User.Password))
                {
                    errorMessage = "Password does not match.";
                    return Page();
                }

                // Get the first user in the list
                User = users.First<User>();

                // proceed to welcome page
                await _context.SaveChangesAsync();
                return Redirect("Welcome/" + User.ID);
            }
            return Page();
        }
        
        public void OnGet()
        {

        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        private static bool ByteArraysEqual(byte[] byteArr1, byte[] byteArr2)
        {
            // If the two arrays are different lengths, return false
            if (byteArr1.Length != byteArr2.Length)
            {
                return false;
            }

            for (int i = 0; i < byteArr1.Length; i++)
            {
                if (byteArr1[i] != byteArr2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }



}
