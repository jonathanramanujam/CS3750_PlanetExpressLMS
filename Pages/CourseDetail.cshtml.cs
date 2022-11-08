using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750_PlanetExpressLMS.Data;
using CS3750_PlanetExpressLMS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System;

namespace CS3750_PlanetExpressLMS.Pages
{
    public class CourseDetailModel : PageModel
    {
        private readonly IAssignmentRepository assignmentRepository;
        private readonly ISubmissionRepository submissionRepository;
        private readonly IEnrollmentRepository enrollmentRepository;

        public CourseDetailModel(IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, IEnrollmentRepository enrollmentRepository)
        {
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            this.enrollmentRepository = enrollmentRepository;
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


        //Chart stuff
        public int[] Grades { get; set; }
        public bool CourseHasGrades;


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

            //Initialize all lists
            courseEnrollments = enrollmentRepository.GetEnrollmentsByCourse(courseID);
            courseAssignments = assignmentRepository.GetAssignmentsByCourse(courseID).ToList();
            courseSubmissions = new Submission[courseAssignments.Count()];
            assignmentHasSubmission = new bool[courseAssignments.Count()];
            if (!user.IsInstructor)
            {
                submissions = submissionRepository.GetStudentSubmissions(user.ID).ToList();
            }
            Grades = new int[12];
            for (int i = 0; i < 12; i++)
            {
                Grades[i] = 0;
            }


            //Calculate and save (if necessary) cumulative grade for every student in the class.
            foreach(Enrollment e in courseEnrollments)
            {

            }
            //Get enrollment
            foreach (Enrollment e in courseEnrollments)
            {
                if (user.ID == e.UserID)
                {
                    enrollment = e;
                }
            }

            //If user is a student, and the course has assignments, check for submissions
            if (courseAssignments.Count() != 0)
            {
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

                    letterGrade = GetLetterGrade(percentGrade);
                }

                //Save the percent grade in the current student's Enrollment object
                if (totalPointsPossible > 0 && enrollment.CumulativeGrade != (decimal)percentGrade)
                {
                    enrollment.CumulativeGrade = (decimal)percentGrade;
                    enrollmentRepository.Update(enrollment);

                }
            }

            //If user is an instructor, get a count of letter grades for every student
            if(user.IsInstructor)
            {
                foreach(Enrollment e in courseEnrollments)
                {
                    GetLetterGrade(e.CumulativeGrade);
                }

                //Then, check if the course has any grades. (If it doesn't, the chart does not display to the instructor.)
                CourseHasGrades = false;
                foreach (Assignment a in courseAssignments)
                {
                    submissions = submissionRepository.GetSubmissionsByAssignment(a.ID).ToList();
                    foreach(Submission s in submissions)
                    {
                        if (s.Grade != null)
                        {
                            CourseHasGrades = true;
                        }
                    }
                }
            }

            return Page();
        }



        public IActionResult OnPost(int courseId)
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
            assignment = assignmentRepository.Add(assignment);

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

            Grades = new int[12];
            for (int i = 0; i < 12; i++)
            {
                Grades[i] = 0;
            }

            return Page();
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

            Grades = new int[12];
            for (int i = 0; i < 12; i++)
            {
                Grades[i] = 0;
            }

            return Redirect("/CourseDetail/" + courseId);
        }



        public string GetLetterGrade(decimal? percentGrade)
        {
            /* Grades array counts how many students have each grade. 
             * Grades[0] - A, Grades [1] - A+, etc. */

            if (percentGrade >= 94)
            {
                letterGrade = "A";
                Grades[0]++;
            }
            else if (percentGrade >= 90)
            {
                letterGrade = "A-";
                Grades[1]++;
            }
            else if (percentGrade >= 87)
            {
                letterGrade = "B+";
                Grades[2]++;
            }
            else if (percentGrade >= 84)
            {
                letterGrade = "B";
                Grades[3]++;
            }
            else if (percentGrade >= 80)
            {
                letterGrade = "B-";
                Grades[4]++;
            }
            else if (percentGrade >= 77)
            {
                letterGrade = "C+";
                Grades[5]++;
            }
            else if (percentGrade >= 74)
            {
                letterGrade = "C";
                Grades[6]++;
            }
            else if (percentGrade >= 70)
            {
                letterGrade = "C-";
                Grades[7]++;
            }
            else if (percentGrade >= 67)
            {
                letterGrade = "D+";
                Grades[8]++;
            }
            else if (percentGrade >= 64)
            {
                letterGrade = "D";
                Grades[9]++;
            }
            else if (percentGrade >= 60)
            {
                letterGrade = "D-";
                Grades[10]++;
            }
            else
            {
                letterGrade = "E";
                Grades[11]++;
            }

            return letterGrade;
        }
    }
}
