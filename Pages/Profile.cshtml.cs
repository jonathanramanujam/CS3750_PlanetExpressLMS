using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using CS3750_PlanetExpressLMS.Data;

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
        public User User { get; set; }

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

            // Saves user info on the profile form
            User = userRepository.GetUser((int)id);

            // 'Refresh' the page
            return Page();
        }

        public async Task<IActionResult> OnGet(int? id)
        {

            //Get user based on id. If no user/id exists, redirect to login.
            User = userRepository.GetUser((int)id);
            if (User == null)
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
                        User.Image = imageUpload;
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
            }
            User = userRepository.Update(User);

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
