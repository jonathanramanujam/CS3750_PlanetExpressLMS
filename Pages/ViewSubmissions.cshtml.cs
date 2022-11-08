using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class ViewSubmissionsModel : PageModel
    {
        private readonly IUserRepository userRepository;
        private readonly ISubmissionRepository submissionRepository;
        private readonly IAssignmentRepository assignmentRepository;

        public ViewSubmissionsModel(IUserRepository userRepository, ISubmissionRepository submissionRepository, IAssignmentRepository assignmentRepository)
        {
            this.userRepository = userRepository;
            this.submissionRepository = submissionRepository;
            this.assignmentRepository = assignmentRepository;
        }

        public User user { get; set; }
        public List<Submission> Submissions { get; set; }

        public Assignment Assignment { get; set; }

        public List<User> UsersWithSubmissions { get; set; }

        public List<bool> SubmissionIsLate { get; set; }

        public bool AnySubmissionsGraded { get; set; }

        public int[] Grades { get; set; }

        public IActionResult OnGet(int assignmentId)
        {
            //Get user from session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            //Instantiate everything
            UsersWithSubmissions = new List<User>();
            Assignment = assignmentRepository.GetAssignment(assignmentId);
            SubmissionIsLate = new List<bool>();
            Grades = new int[5];

            //Only instructors can view this page.
            if(!user.IsInstructor)
            {
                return NotFound();
            }

            Submissions = submissionRepository.GetSubmissionsByAssignment(assignmentId).ToList();

            foreach(var s in Submissions)
            {
                //Create a list of users corresponding to our submissions
                UsersWithSubmissions.Add(userRepository.GetUser(s.UserID));
                if (s.SubmissionTime > Assignment.CloseDateTime)
                {
                    SubmissionIsLate.Add(true);
                }
                else
                {
                    SubmissionIsLate.Add(false);
                }

                //Chart stuff
                if (s.Grade != null)
                {
                    //Track if any assignment has been graded. (If none have, the grade chart won't display.)
                    AnySubmissionsGraded = true;
                    /*Track grades to display in the chart.
                     * Grades[0] - A
                     * Grades[1] - B
                     * Grades[2] - C
                     * Grades[3] - D
                     * Grades[4] - F */
                    decimal PercentGrade = ((decimal)s.Grade / (decimal)Assignment.PointsPossible) * 100;
                    if(PercentGrade >= 90)
                    {
                        Grades[0]++;
                    }
                    else if(PercentGrade >= 80)
                    {
                        Grades[1]++;
                    }
                    else if(PercentGrade >= 70)
                    {
                        Grades[2]++;
                    }
                    else if(PercentGrade >= 60)
                    {
                        Grades[3]++;
                    }
                    else
                    {
                        Grades[4]++;
                    }
                }
                //End chart stuff
            }

            return Page();
        }
    }
}
