using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class ViewSubmissionsModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ISubmissionRepository submissionRepository;
        private readonly IAssignmentRepository assignmentRepository;
        public readonly INotificationRepository notificationRepository;

        public ViewSubmissionsModel(IUserRepository userRepository, ISubmissionRepository submissionRepository, IAssignmentRepository assignmentRepository, INotificationRepository notificationRepository)
        {
            this.userRepository = userRepository;
            this.submissionRepository = submissionRepository;
            this.assignmentRepository = assignmentRepository;
            this.notificationRepository = notificationRepository;
        }

        public User user { get; set; }
        public List<Submission> Submissions { get; set; }

        public Assignment Assignment { get; set; }

        public List<User> UsersWithSubmissions { get; set; }

        public List<bool> SubmissionIsLate { get; set; }

        public List<Notification> notifications { get; set; }

        public IActionResult OnGet(int assignmentId)
        {
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);
            user = session.GetUser();

            notifications = notificationRepository.GetNotifications(user.ID);

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            UsersWithSubmissions = new List<User>();
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            SubmissionIsLate = new List<bool>();

            if(!user.IsInstructor)
            {
                return NotFound();
            }
            Submissions = submissionRepository.GetSubmissionsByAssignment(assignmentId).ToList();

            //Create a list of users corresponding to our submissions
            foreach(var s in Submissions)
            {
                UsersWithSubmissions.Add(userRepository.GetUser(s.UserID));
                if (s.SubmissionTime > Assignment.CloseDateTime)
                {
                    SubmissionIsLate.Add(true);
                }
                else
                {
                    SubmissionIsLate.Add(false);
                }
            }

            return Page();
        }
    }
}
