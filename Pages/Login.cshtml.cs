using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

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
        public User user { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Access the current session
            // Run this for every request
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Get a list of users
            var users = userRepository.GetAllUsers();

            // if Email and password entries are not empty
            if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Password))
            {
                // look for Email in database
                users = users.Where(c => c.Email == user.Email);
                if (users.Count() == 0)
                {
                    errorMessage = "Email does not exist.";
                    return Page();
                }

                // If the password does not match, return the page with an error
                if (!VerifyHashedPassword(users.First().Password, user.Password))
                {
                    errorMessage = "Password does not match.";
                    return Page();
                }

                // Get the first user in the list
                user = users.First();

                // If the user does not exist, return not found
                if (user == null) { return NotFound(); }

                // Add user to session
                session.SetUser(user);

                // proceed to welcome page
                return Redirect("Dashboard/");
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
