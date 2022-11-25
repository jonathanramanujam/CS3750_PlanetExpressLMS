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
        public readonly INotificationRepository notificationRepository;
        private readonly IEnrollmentRepository enrollmentRepository;

        public CourseDetailModel(IAssignmentRepository assignmentRepository, ISubmissionRepository submissionRepository, INotificationRepository notificationRepository, IEnrollmentRepository enrollmentRepository)
        {
            this.assignmentRepository = assignmentRepository;
            this.submissionRepository = submissionRepository;
            this.notificationRepository = notificationRepository;
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

        public List<Enrollment> enrollments;

        public Notification notification { get; set; }

        public List<Notification> notifications { get; set; }

        public List<Submission> submissions { get; set; }

        public List<Assignment> courseAssignments { get; set; }

        public List<Enrollment> courseEnrollments { get; set; }

        //Used to display assignment grades
        public Submission[] courseSubmissions { get; set; }


        //Tracks if each assignment has a submission - if it does, page displays "Re-submit"
        public bool[] assignmentHasSubmission { get; set; }



        //Cumulative course grade calculations - for charts, etc.
        //Store percent grade for every student in the class
        public List<decimal> AllStudentGrades { get; set; }

        //Store the letter grade for the current student
        public string? letterGrade { get; set; }
        //Store the percent grade for the current student
        public decimal percentGrade { get; set; }


        //Count the number of students who have each grade
        public int[] Grades { get; set; }


        public async Task<IActionResult> OnGetAsync (int courseID)
        {
            // Access the current session
            PlanetExpressSession session = new PlanetExpressSession(HttpContext);

            // Make sure a user is logged in
            user = session.GetUser();

            notifications = notificationRepository.GetNotifications(user.ID);

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

            //Get all assignments for this course
            courseAssignments = assignmentRepository.GetAssignmentsByCourse(courseID).ToList();

            //Get cumulative grades for every enrollment in the course.
            //Stored in list AllStudentGrades
            GetAllGrades(courseID);

            //Student stuff
            //Get user enrollment
            foreach (Enrollment e in courseEnrollments)
            {
                if (user.ID == e.UserID)
                {
                    enrollment = e;
                }
            }

            if (courseAssignments.Count() > 0)
            {
                if (!user.IsInstructor)
                {

                    //Calculate percent and letter grade for the user
                    if (enrollment.TotalPointsPossible > 0)
                    {
                        percentGrade = CalculateCumulativeGrade(enrollment.TotalPointsEarned, enrollment.TotalPointsPossible);
                        letterGrade = GetLetterGrade(percentGrade);
                    }

                    //Get all submissions by the user
                    submissions = submissionRepository.GetStudentSubmissions(user.ID).ToList();

                    //Check which assignments have submissions. Used to mark each assignment with "Re-submit"
                    TrackUserSubmissions();
                }
                //End student stuff

                //Instructor stuff
                else
                {
                    //Count the number of grades in each category
                    //Initialize grade count array
                    InitializeGradeCount();

                    //Get counts based on AllStudentGrades
                    foreach(decimal d in AllStudentGrades)
                    {
                        GetGradeCount(d);
                    }
                }
                //End instructor stuff

            }//End if courseAssignments.Count() > 0

            return Page();
        }


        //Create a new assignment based on information entered by the instructor.
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
            InitializeGradeCount();
            GetAllGrades(courseId);

            //Add notifications for students enrolled in course
            enrollments = enrollmentRepository.GetStudentsEnrolled(courseId);

            foreach (var student in enrollments)
            {
                if (student.UserID != user.ID)
                {
                    notification = new Notification();
                    notification.Title = course.Department.ToString() + " " + course.CourseNumber.ToString() + " " + assignment.Name.ToString() + " Created";
                    notification.UserID = student.UserID;
                    notificationRepository.Add(notification);
                }
            }

            return Redirect("/CourseDetail/" + courseId);
        }



        //Delete an assignment of the instructor's choice. (All submissions for the assignment will also be deleted.)
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

            //All enrollments that have been graded for this assignment need to have their total points possible and total points earned decreased.
            var assignmentSubmissions = submissionRepository.GetSubmissionsByAssignment(assignmentId);
            var assignmentToDelete = assignmentRepository.GetAssignment(assignmentId);
            List<Enrollment> enrollmentsToModify = new List<Enrollment>();

            foreach (Submission s in assignmentSubmissions)
            {
                if (s.Grade != null)
                { 
                    var tempEnrollmentList = enrollmentRepository.GetUserEnrollments(s.UserID);
                    foreach (var enrollment in tempEnrollmentList)
                    {
                        if (enrollment.CourseID == courseId)
                        {
                            enrollment.TotalPointsPossible -= assignmentToDelete.PointsPossible;
                            enrollment.TotalPointsEarned -= (decimal)s.Grade;
                            enrollmentsToModify.Add(enrollment);
                        }
                    }

                }
            }

            //Getting and saving enrollments in the dbcontext requires them to be in different loops
            foreach (Enrollment e in enrollmentsToModify)
            {
                //If this part is needed, something went wrong.
                if (e.TotalPointsEarned < 0)
                {
                    e.TotalPointsEarned = 0;
                }
                if (e.TotalPointsPossible < 0)
                {
                    e.TotalPointsPossible = 0;
                }

                enrollmentRepository.Update(e);
            }

            //End enrollment modification.

            InitializeGradeCount();
            GetAllGrades(courseId);

            //Finally, delete the assignment.
            assignmentRepository.Delete(assignmentId);

            return Redirect("/CourseDetail/" + courseId);
        }

        public void GetAllGrades(int courseID)
        {
            courseEnrollments = enrollmentRepository.GetEnrollmentsByCourse(courseID);
            AllStudentGrades = new List<decimal>();

            foreach (Enrollment e in courseEnrollments)
            {
                if (e.TotalPointsPossible > 0) //Only count the enrollment if student has had at least one assignment graded
                {
                    var grade = CalculateCumulativeGrade(e.TotalPointsEarned, e.TotalPointsPossible);
                    AllStudentGrades.Add(grade);
                }
            }
        }

        //Check which assignments have submissions. Used to mark each assignment with "Re-submit"
        public void TrackUserSubmissions()
        {
            assignmentHasSubmission = new bool[courseAssignments.Count()];
            courseSubmissions = new Submission[courseAssignments.Count()];

            for (int i = 0; i < courseAssignments.Count(); i++)
            {
                foreach (Submission s in submissions)
                {
                    if (s.AssignmentID == courseAssignments.ElementAt(i).ID)
                    {
                        assignmentHasSubmission[i] = true;
                        courseSubmissions[i] = s;
                        break;
                    }
                    else
                    {
                        assignmentHasSubmission[i] = false;
                        var spacer = new Submission();
                        courseSubmissions[i] = spacer;
                    }
                }
            }
        }

        public void InitializeGradeCount()
        {
            Grades = new int[12];
            for (int i = 0; i < 12; i++)
            {
                Grades[i] = 0;
            }
        }

        public decimal CalculateCumulativeGrade(decimal TotalPointsEarned, decimal TotalPointsPossible)
        {
            return Decimal.Round((TotalPointsEarned / TotalPointsPossible) * 100, 2);
        }

        public string GetLetterGrade(decimal? pGrade)
        {
            string lGrade; //letter grade to return

            if (pGrade >= 94)
            {
                lGrade = "A";
            }
            else if (pGrade >= 90)
            {
                lGrade = "A-";
            }
            else if (pGrade >= 87)
            {
                lGrade = "B+";
            }
            else if (pGrade >= 84)
            {
                lGrade = "B";
            }
            else if (pGrade >= 80)
            {
                lGrade = "B-";
            }
            else if (pGrade >= 77)
            {
                lGrade = "C+";
            }
            else if (pGrade >= 74)
            {
                lGrade = "C";
            }
            else if (pGrade >= 70)
            {
                lGrade = "C-";
            }
            else if (pGrade >= 67)
            {
                lGrade = "D+";
            }
            else if (pGrade >= 64)
            {
                lGrade = "D";
            }
            else if (pGrade >= 60)
            {
                lGrade = "D-";
            }
            else
            {
                lGrade = "E";
            }

            return lGrade;
        }

        public void GetGradeCount(decimal pGrade)
        {

            if (pGrade >= 94)
            {
                Grades[0]++;
            }
            else if (pGrade >= 90)
            {
                Grades[1]++;
            }
            else if (pGrade >= 87)
            {
                Grades[2]++;
            }
            else if (pGrade >= 84)
            {
                Grades[3]++;
            }
            else if (pGrade >= 80)
            {
                Grades[4]++;
            }
            else if (pGrade >= 77)
            {
                Grades[5]++;
            }
            else if (pGrade >= 74)
            {
                Grades[6]++;
            }
            else if (pGrade >= 70)
            {
                Grades[7]++;
            }
            else if (pGrade >= 67)
            {
                Grades[8]++;
            }
            else if (pGrade >= 64)
            {
                Grades[9]++;
            }
            else if (pGrade >= 60)
            {
                Grades[10]++;
            }
            else
            {
                Grades[11]++;
            }
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
            return RedirectToPage("CourseDetail");
        }
    }
}
