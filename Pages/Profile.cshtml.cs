using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using CS3750_PlanetExpressLMS.Data;
using System.Text.Json;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public ProfileModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public BufferedImageUpload FileUpload { get; set; }

        /// <summary>
        /// User can only edit their profile if they hit the edit button
        /// </summary>
        public bool isEditMode = false;

        public bool alertMsg = false;

        /// <summary>
        /// Function to handle edit profile event
        /// </summary>
        public async Task<IActionResult> OnPostToggleEdit(int? id)
        {
            //Allow the UI to be edited on
            isEditMode = !isEditMode;

            // Get user from session
            user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));

            // 'Refresh' the page
            return Page();
        }

        public async Task<IActionResult> OnGet(int id)
        {
            // Get the user
            user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("user"));

            // Make sure a user is logged in
            if (user == null)
            {
                return Redirect("Login/");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (FileUpload.FormFile != null)
            {
                // Convert the user's uploaded image to a byte array, for database storage
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await FileUpload.FormFile.CopyToAsync(memoryStream);

                    //Upload the file if less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        byte[] imageUpload = memoryStream.ToArray();
                        user.Image = imageUpload;
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
            }

            // Update the user
            user = userRepository.Update(user);

            // Update the session
            HttpContext.Session.SetString("user", JsonSerializer.Serialize(user));

            // Notifies the user that they're updates have been saved
            alertMsg = !alertMsg;

            return Page();
        }

        public class BufferedImageUpload
        {
            [Display(Name = "Profile Image")]
            public IFormFile FormFile { get; set; }
        }
    }
}
