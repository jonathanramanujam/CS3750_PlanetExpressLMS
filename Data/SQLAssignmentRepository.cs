using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLAssignmentRepository : IAssignmentRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        private readonly ISubmissionRepository submissionRepository;
        public SQLAssignmentRepository(CS3750_PlanetExpressLMSContext context, ISubmissionRepository submissionRepository)
        {
            this.context = context;
            this.submissionRepository = submissionRepository;
        }

        public Assignment Add(Assignment newAssignment)
        {
            context.Assignment.Add(newAssignment);
            context.SaveChanges();
            return newAssignment;
        }

        public Assignment Delete(int id)
        {
            Assignment assignment = context.Assignment.Find(id);
            //Get all submissions for this assignment.
            var assignmentSubmissions = submissionRepository.GetSubmissionsByAssignment(id).ToList();
            foreach (var s in assignmentSubmissions)
            {
                //Delete submission in database.
                submissionRepository.Delete(s.ID);
            }
            //Finally, delete the assignment
            if (assignment != null)
            {
                context.Assignment.Remove(assignment);
                context.SaveChanges();
            }
            return assignment;
        }

        public IEnumerable<Assignment> GetAllAssignments()
        {
            return context.Assignment;
        }

        public Assignment GetAssignment(int id)
        {
            return context.Assignment.Find(id);
        }

        public Assignment Update(Assignment updatedAssignment)
        {
            var ass = context.Assignment.Attach(updatedAssignment);
            //Right now, this updates the ENTIRE Assignment object.
            //If you don't want that to happen, remove the below statement:
            ass.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //Then add statements here like, for example:
            //context.Entry(updatedAssignment).Property("CourseID").IsModified = true;
            //And ONLY those attributes will be updated.
            context.SaveChanges();
            //Then, replace the return statement with this to 'reset' the context and get good values:
            /*context.Entry(updatedAssignment).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
              var assignment = context.Enrollment.Find(updatedAssignment.ID);
              return assignment;*/
            return updatedAssignment;
        }

        public List<Assignment> GetAssignmentsByCourse(int courseId)
        {
            var assignments = GetAllAssignments();
            assignments = assignments.Where(a => a.CourseID == courseId);
            return assignments.ToList();
        }
        public List<Assignment> GetStudentAssignments(int userID, List<Course> courses)
        {
            List<Assignment> retAssignments = new List<Assignment>();

            foreach (var course in courses)
            {
                if (GetAssignmentsByCourse(course.ID) != null)
                {
                    foreach(var assignment in GetAssignmentsByCourse(course.ID).ToList<Assignment>())
                    {
                        retAssignments.Add(assignment);
                    }
                }
            }
            return retAssignments.OrderBy(x => x.CloseDateTime).ToList();
        }

        public List<Assignment> GetInstructorAssignments(int userID, List<Course> courses)
        {
            List<Assignment> retAssignments = new List<Assignment>();

            foreach (var course in courses)
            {
                if (GetAssignmentsByCourse(course.ID) != null)
                {
                    foreach (var assignment in GetAssignmentsByCourse(course.ID).ToList<Assignment>())
                    {
                        retAssignments.Add(assignment);
                    }
                }
            }
            return retAssignments.OrderBy(x => x.CloseDateTime).ToList();
        }
    }
}
