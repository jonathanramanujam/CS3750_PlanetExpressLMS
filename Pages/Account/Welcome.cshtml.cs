using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class WelcomeModel : PageModel
    {
        private readonly CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext _context;

        public WelcomeModel(CS3750_PlanetExpressLMS.Data.CS3750_PlanetExpressLMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public BufferedImageUpload FileUpload { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = await _context.User.FirstOrDefaultAsync(c => c.ID == id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            // Otherwise, return the page
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            using (var memoryStream = new MemoryStream())
            {
                await FileUpload.FormFile.CopyToAsync(memoryStream);

                //Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    var imageUpload = memoryStream.ToArray();
                    User.Image = imageUpload;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }
            return Page();
        }
    }

    public class BufferedImageUpload
    {
        [Required]
        [Display(Name ="File")]
        public IFormFile FormFile { get; set; }
    }
}
