using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLCourseRepository : ICourseRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public readonly IAssignmentRepository assignmentRepository;
        public SQLCourseRepository(CS3750_PlanetExpressLMSContext context, IAssignmentRepository assignmentRepository)
        {
            this.context = context;
            this.assignmentRepository = assignmentRepository;
        }

        public Course Add(Course newCourse)
        {
            context.Course.Add(newCourse);
            context.SaveChanges();
            return newCourse;
        }

        public Course Delete(int id)
        {
            Course course = context.Course.Find(id);
            //Get all submissions for this assignment
            var courseAssignments = assignmentRepository.GetAssignmentsByCourse(id);
            foreach (var courseAssignment in courseAssignments)
            {
                //Delete assignment in database
                assignmentRepository.Delete(courseAssignment.ID);
            }

            if (course != null)
            {
                context.Course.Remove(course);
                context.SaveChanges();
            }
            return course;
        }

        public List<Course> GetAllCourses()
        {
            return context.Course.ToList();
        }

        public List<Course> GetInstructorCourses(int id)
        {
            var userCourses = GetAllCourses();
            userCourses = userCourses.Where(c => c.UserID == id).ToList();
            return userCourses;
        }

        public List<Course> GetStudentCourses(int id)
        {
            List<Enrollment> studEnrollments = context.Enrollment.Where(e => e.UserID == id).ToList<Enrollment>();
            List<Course> retCourses = new List<Course>();
            foreach(var enrollment in studEnrollments)
            {
                var courseID = enrollment.CourseID;
                if(GetCourse(courseID) != null)
                {
                    retCourses.Add(GetCourse(courseID));
                }
            }
            return retCourses;
        }

        public Course GetCourse(int id)
        {
            return context.Course.Find(id);
        }

        public Course Update(Course updatedCourse)
        {
            var course = context.Course.Attach(updatedCourse);
            //Right now, this updates the ENTIRE Course object.
            //If you don't want that to happen, remove the below statement:
            course.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //Then add statements here like, for example:
            //context.Entry(updatedCourse).Property("CourseName").IsModified = true;
            //And ONLY those attributes will be updated.
            context.SaveChanges();
            //Then, replace the return statement with this to 'reset' the context and get good values:
          /*context.Entry(updatedCourse).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            var Course = context.Course.Find(updatedCourse.ID);
            return Course;*/
            return updatedCourse;
         }
    }
}
