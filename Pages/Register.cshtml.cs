using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Security.Cryptography;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserRepository userRepository;
        public RegisterModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public string errorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate that the values will work for the User model. If not, reload the page with the validation summary. This only happens OnPost.
            if (!ModelState.IsValid) { return Page(); }

            var Email = userRepository.GetAllUsers();
            Email = Email.Where(c => c.Email == user.Email);

            // If the Email already exists, reload the register page
            if (Email.Count() != 0)
            {
                errorMessage = $"The Email '{Email.First().Email}' already exists.";
                return Page();
            }

            // Hash the user's password
            user.Password = HashPassword(user.Password);

            // Else, add the new user User to the database
            userRepository.Add(user);

            // Then redirect to the user's welcome page
            return Redirect("dashboard/" + user.ID);
        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }
    }
}
