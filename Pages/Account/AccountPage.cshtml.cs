using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages.Account
{
    public class AccountModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public AccountModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public Payment Payment { get; set; }

        [BindProperty] 
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            // If no id was passed, return not found
            if (id == null) { return NotFound(); }

            // Look up the user based on the id
            User = userRepository.GetUser((int)id);

            // If the user does not exist, return not found
            if (User == null) { return NotFound(); }

            HttpClient client = new HttpClient();

            //client.PostAsync(get request); token request

            //client.PostAsync(get request); payment request

            // Otherwise, return the page
            return Page();
        }

    }
}
