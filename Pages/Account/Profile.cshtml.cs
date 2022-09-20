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
        public User CurrUser { get; set; }

        [BindProperty]
        public BufferedImageUpload FileUpload { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            //Get user based on id. If no user/id exists, redirect to login.
            CurrUser = await _context.User.FirstOrDefaultAsync(u => u.ID == id);
            if (CurrUser == null)
            {
                return Redirect("Login/");
            }

            

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _context.Attach(CurrUser);
            _context.Entry(CurrUser).Property(u => u.Email).IsModified = true;
            _context.Entry(CurrUser).Property(u => u.FirstName).IsModified = true;
            _context.Entry(CurrUser).Property(u => u.LastName).IsModified = true;
            _context.Entry(CurrUser).Property(u => u.Bio).IsModified = true;

            await _context.SaveChangesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await FileUpload.FormFile.CopyToAsync(memoryStream);

                //Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    // Get the User and update their image
                    CurrUser = await _context.User.FirstOrDefaultAsync(c => c.ID == CurrUser.ID);
                    byte[] imageUpload = memoryStream.ToArray();
                    CurrUser.Image = imageUpload;
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
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
