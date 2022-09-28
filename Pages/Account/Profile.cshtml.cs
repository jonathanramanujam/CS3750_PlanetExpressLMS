using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using CS3750_PlanetExpressLMS.Data;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public ProfileModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public User User { get; set; }

        [BindProperty]
        public BufferedImageUpload FileUpload { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            //Get user based on id. If no user/id exists, redirect to login.
            User = userRepository.GetUser(id);
            if (User == null)
            {
                return Redirect("Login/");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(User user)
        {
            // Convert the user's uploaded image to a byte array, for database storage
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (FileUpload.FormFile != null)
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

            User = userRepository.Update(user);

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
