using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLEnrollmentRepository : IEnrollmentRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public SQLEnrollmentRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }

        public Enrollment Add(Enrollment newEnrollment)
        {
            context.Enrollment.Add(newEnrollment);
            context.SaveChanges();
            return newEnrollment;
        }

        public Enrollment Delete(int id)
        {
            Enrollment en = context.Enrollment.Find(id);
            if(en != null)
            {
                context.Enrollment.Remove(en);
                context.SaveChanges();
            }
            return en;
        }

        public IEnumerable<Enrollment> GetAllEnrollments()
        {
            return context.Enrollment;
        }

        public List<Enrollment> GetUserEnrollments(int userId)
        {
            var userEnrollments = GetAllEnrollments();
            userEnrollments = userEnrollments.Where(c => c.UserID == userId);
            return userEnrollments.ToList();
        }

        public List<Enrollment> GetEnrollmentsByCourse(int courseID)
        {
            var userEnrollments = GetAllEnrollments();
            userEnrollments = userEnrollments.Where(c => c.CourseID == courseID);
            return userEnrollments.ToList();
        }

        public Enrollment GetEnrollment(int id)
        {
            return context.Enrollment.Find(id);
        }

        public Enrollment Update(Enrollment updatedEnrollment)
        {
            var en = context.Enrollment.Attach(updatedEnrollment);
            //Right now, this updates the ENTIRE Course object.
            //If you don't want that to happen, remove the below statement:
            en.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //Then add statements here like, for example:
            //context.Entry(updatedEnrollment).Property("UserID").IsModified = true;
            //And ONLY those attributes will be updated.
            context.SaveChanges();
            //Then, replace the return statement with this to 'reset' the context and get good values:
            /*context.Entry(updatedEnrollment).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
              var enrollment = context.Enrollment.Find(updatedEnrollment.ID);
              return enrollment;*/
            return updatedEnrollment;
        }

        public List<Enrollment> GetStudentsEnrolled(int courseID)
        {
            var studentEnrolled = GetAllEnrollments();
            studentEnrolled = studentEnrolled.Where(c => c.CourseID == courseID);
            return studentEnrolled.ToList<Enrollment>();
        }
    }
}
