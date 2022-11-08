using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

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

        public async Task<IActionResult> OnGet()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            return Page();
        }

        /// <summary>
        /// Function to handle edit profile event
        /// </summary>
        public async Task<IActionResult> OnPostToggleEdit()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            //Allow the UI to be edited on
            isEditMode = !isEditMode;

            // 'Refresh' the page
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            User sessionUser = session.GetUser();

            if (sessionUser == null)
            {
                return RedirectToPage("Login");
            }

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
            session.SetUser(user);

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
