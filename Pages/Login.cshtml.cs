using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public LoginModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [BindProperty]
        public string errorMessage { get; set; }

        [BindProperty] //Allows us to retrieve User values, and convert from strings to .NET types. Automates and reduces error
        public User User { get; set; }

        public async Task<IActionResult> OnPostAsync(int ID)
        {
            //if (!ModelState.IsValid) { return Page(); }

            // Get a list of users
            var users = userRepository.GetAllUsers();

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

                // If the password does not match, return the page with an error
                if (!VerifyHashedPassword(users.First().Password, User.Password))
                {
                    errorMessage = "Password does not match.";
                    return Page();
                }

                // Get the first user in the list
                User = users.First();

                // proceed to welcome page
                return Redirect("Dashboard/" + User.ID);
            }
            return Page();
        }

        public void OnGet()
        {
            //Clear session data
            HttpContext.Session.Clear();
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
            if (src.Length != 0x31 || src[0] != 0)
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
