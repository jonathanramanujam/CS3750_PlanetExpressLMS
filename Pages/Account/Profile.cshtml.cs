using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext _context;
        public ProfileModel(CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext context)
        {
            _context = context;
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
            User = await _context.User.FirstOrDefaultAsync(u => u.ID == id);

            // 'Refresh' the page
            return Page();
        }

        public async Task<IActionResult> OnGet(int? id)
        {

            //Get user based on id. If no user/id exists, redirect to login.
            User = await _context.User.FirstOrDefaultAsync(u => u.ID == id);
            if (User == null)
            {
                return Redirect("Login/");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if(FileUpload.FormFile != null)
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


      

            /*Using context.Attach just ensures that only the attributes we intend to 
             * update actually get updated. This prevents the app from attempting to
             * insert NULL into user ID and stuff like that.*/
            _context.Attach(User);
            _context.Entry(User).Property(u => u.FirstName).IsModified = true;
            _context.Entry(User).Property(u => u.LastName).IsModified = true;
            _context.Entry(User).Property(u => u.Bio).IsModified = true;
            _context.Entry(User).Property(u => u.Image).IsModified = true;
            _context.Entry(User).Property(u => u.Address1).IsModified = true;
            _context.Entry(User).Property(u => u.Address2).IsModified = true;
            _context.Entry(User).Property(u => u.City).IsModified = true;
            _context.Entry(User).Property(u => u.State).IsModified = true;
            _context.Entry(User).Property(u => u.ZipCode).IsModified = true;
            _context.Entry(User).Property(u => u.Link1).IsModified = true;
            _context.Entry(User).Property(u => u.Link2).IsModified = true;
            _context.Entry(User).Property(u => u.Link3).IsModified = true;

            await _context.SaveChangesAsync();

            // Notifies the user that they're updates have been saved
            alertMsg = !alertMsg;

            return Page();
        }
    }

    public class BufferedImageUpload
    {
        [Required]
        [Display(Name = "Profile Image")]
        public IFormFile FormFile { get; set; }
    }
}
