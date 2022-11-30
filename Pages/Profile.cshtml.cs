using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CS3750_PlanetExpressLMS.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;
using AngleSharp.Dom;
using System.Web.WebPages;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly IUserRepository userRepository;
        public readonly INotificationRepository notificationRepository;
        public ProfileModel(IUserRepository userRepository, INotificationRepository notificationRepository)
        {
            this.userRepository = userRepository;
            this.notificationRepository = notificationRepository;
        }
        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public BufferedImageUpload FileUpload { get; set; }

        public List<Notification> notifications { get; set; }

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

            notifications = notificationRepository.GetNotifications(user.ID);

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

            Uri[] uriResult = new Uri[3];

            if (!IsValidURL(user.Link1, out uriResult[0]))
            {
                user.Link1 = sessionUser.Link1;
            }
            else
            {
                user.Link1 = uriResult[0].ToString();
            }

            if (!IsValidURL(user.Link2, out uriResult[1]))
            {
                user.Link2 = sessionUser.Link2;
            }
            else
            {
                user.Link2 = uriResult[1].ToString();
            }

            if (!IsValidURL(user.Link3, out uriResult[2]))
            {
                user.Link3 = sessionUser.Link3;
            }
            else
            {
                user.Link3 = uriResult[2].ToString();
            }




            // Update the user
            user = userRepository.Update(user);

            // Update the session
            session.SetUser(user);

            // Notifies the user that they're updates have been saved
            alertMsg = !alertMsg;

            return Page();
        }

        #region Valid URL
        /// <summary>
        /// Checks if a user typed in a valid url. If its a valid link but it doesn't
        /// have an http(s) at the begining, this function appends the url with it
        /// </summary>
        /// <param name="s"></param>
        /// <param name="resultURI"></param>
        /// <returns></returns>
        public static bool IsValidURL(string s, out Uri resultURI)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // String must not be null or empty or else it'll crash regex method
            if (!string.IsNullOrEmpty(s))
            {
                // Checks if url is valid
                if (Rgx.IsMatch(s))
                {
                    // If url doesn't have http(s), append it
                    if (!Regex.IsMatch(s, @"^https?:\/\/", RegexOptions.IgnoreCase))
                        s = "http://" + s;

                    if (Uri.TryCreate(s, UriKind.Absolute, out resultURI))
                        return (resultURI.Scheme == Uri.UriSchemeHttp ||
                                resultURI.Scheme == Uri.UriSchemeHttps);
                }
            }

            resultURI = null;
            return false;

        }
        #endregion

        public class BufferedImageUpload
        {
            [Display(Name = "Profile Image")]
            public IFormFile FormFile { get; set; }
        }

        public async Task<IActionResult> OnPostClearNotification(int id)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            notificationRepository.Delete(id);
            return RedirectToPage("Profile");
        }
    }
}
