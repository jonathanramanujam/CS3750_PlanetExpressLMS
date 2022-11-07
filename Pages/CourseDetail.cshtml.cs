using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class CourseDetailModel : PageModel
    {
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;
        private readonly IEnrollmentRepository enrollmentRepository;
        private IWebHostEnvironment _environment;

        public CourseDetailModel(IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, IEnrollmentRepository enrollmentRepository, IWebHostEnvironment environment)
        {
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            this.enrollmentRepository = enrollmentRepository;
            _environment = environment;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public Course course { get; set; }

        [BindProperty]
        public Assignment assignment { get; set; }

        public Enrollment enrollment { get; set; }



        //Lists and arrays to help with calculations
        public List<Assignment> assignments;

        public List<Submission> submissions { get; set; }

        public Submission[] courseSubmissions { get; set; }

        public List<Assignment> courseAssignments { get; set; }

        public List<Enrollment> courseEnrollments { get; set; }

        public bool[] assignmentHasSubmission { get; set; }



        //Cumulative course grade calculations
        public int totalPointsPossible { get; set; }
        public decimal? totalPointsEarned { get; set; }

        public decimal? percentGrade { get; set; }

        public string? letterGrade { get; set; }


        public async Task<IActionResult> OnGetAsync (int courseID)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            // Get courses from the session
            List<Course> courses = session.GetCourses();

            foreach (Course _course in courses)
            {
                if (_course.ID == courseID)
                {
                    course = _course;
                }
            }

            if (course == null) { return NotFound(); }

            //Get all enrollments for this course
            courseEnrollments = enrollmentRepository.GetEnrollmentsByCourse(courseID);

            // Check for existing assignments for this course
            courseAssignments = assignmentRepository.GetAssignmentsByCourse(courseID).ToList();
            //Initialize submission list for this course
            courseSubmissions = new Submission[courseAssignments.Count()];

            //If user is a student, and the course has assignments, check for submissions
            if (!user.IsInstructor && courseAssignments.Count() != 0)
            {
                submissions = submissionRepository.GetStudentSubmissions(user.ID).ToList();

                //Track, for each assignment, if the student has submitted it
                assignmentHasSubmission = new bool[courseAssignments.Count()];

                for (int i = 0; i < courseAssignments.Count(); i++)
                {
                    foreach (Submission submission in submissions)
                    {
                        if (submission.AssignmentID == courseAssignments.ElementAt(i).ID)
                        {
                            assignmentHasSubmission[i] = true;
                            courseSubmissions[i] = submission;
                            break;
                        }
                        else
                        {
                            assignmentHasSubmission[i] = false;
                            var spacer = new Submission(); //Pad the courseSubmissions list so the indexes match and are easier to access
                            courseSubmissions[i] = spacer;
                        }
                    }
                }


                //Get total points possible - but only for assignments that have been graded
                //Also get total points earned by the student

                totalPointsEarned = 0;
                totalPointsPossible = 0;

                for (int i = 0; i < courseAssignments.Count(); i++)
                {
                    if (courseSubmissions[i] != null && courseSubmissions[i].Grade != null)
                    {
                        totalPointsEarned += courseSubmissions[i].Grade;
                        totalPointsPossible += courseAssignments[i].PointsPossible;
                    }
                }

                //Calculate letter grades based on CS3750 grading scheme
                if (totalPointsPossible > 0)
                {
                    percentGrade = Decimal.Round(((decimal)(totalPointsEarned / (decimal?)totalPointsPossible) * 100), 2);

                    if (percentGrade >= 94)
                    {
                        letterGrade = "A";
                    }
                    else if (percentGrade >= 90)
                    {
                        letterGrade = "A-";
                    }
                    else if (percentGrade >= 87)
                    {
                        letterGrade = "B+";
                    }
                    else if (percentGrade >= 84)
                    {
                        letterGrade = "B";
                    }
                    else if (percentGrade >= 80)
                    {
                        letterGrade = "B-";
                    }
                    else if (percentGrade >= 77)
                    {
                        letterGrade = "C+";
                    }
                    else if (percentGrade >= 74)
                    {
                        letterGrade = "C";
                    }
                    else if (percentGrade >= 70)
                    {
                        letterGrade = "C-";
                    }
                    else if (percentGrade >= 67)
                    {
                        letterGrade = "D+";
                    }
                    else if (percentGrade >= 64)
                    {
                        letterGrade = "D";
                    }
                    else if (percentGrade >= 60)
                    {
                        letterGrade = "D-";
                    }
                    else
                    {
                        letterGrade = "E";
                    }
                }

                    //Save the percent grade in the current student's Enrollment object
                    foreach (Enrollment e in courseEnrollments)
                    {
                        if (user.ID == e.UserID)
                        {
                            enrollment = e;
                        }
                    }

                if (totalPointsPossible > 0 && enrollment.CumulativeGrade != (decimal)percentGrade)
                {
                    enrollment.CumulativeGrade = (decimal)percentGrade;
                    enrollmentRepository.Update(enrollment);

                }
            }
            return Page();
        }

        public IActionResult OnPostCreate(int courseId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            assignment.CourseID = courseId;

            //Create a new assignment
            assignmentRepository.Add(assignment);

            // Get courses from the session
            List<Course> courses = session.GetCourses();

            foreach (Course _course in courses)
            {
                if (_course.ID == courseId)
                {
                    course = _course;
                }
            }

            courseAssignments = assignmentRepository.GetAssignmentsByCourse(course.ID).ToList();

            return Redirect("/CourseDetail/" + courseId);
        }

        public IActionResult OnPostDelete(int assignmentId, int courseId)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            if (user == null)
            {
                return RedirectToPage("Login");
            }

            //Finally, delete the assignment.
            assignmentRepository.Delete(assignmentId);

            return Redirect("/CourseDetail/" + courseId);
        }
    }
}
