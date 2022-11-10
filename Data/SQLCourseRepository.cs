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
        public readonly IEnrollmentRepository enrollmentRepository;
        public SQLCourseRepository(CS3750_PlanetExpressLMSContext context, IAssignmentRepository assignmentRepository, IEnrollmentRepository enrollmentRepository)
        {
            this.context = context;
            this.assignmentRepository = assignmentRepository;
            this.enrollmentRepository = enrollmentRepository;
        }

        public SQLCourseRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }

        public List<Course> filteredCourses(string depCode, string searchName)
        {
            var filtered = GetAllCourses();
            if(searchName != "")
            {
                filtered = filtered.Where(f => f.CourseName.Contains(searchName)).ToList();
            }
            
            if(depCode != "")
            {
                filtered = filtered.Where(f => f.Department == depCode).ToList();
            }
            
            return filtered;
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
            //Get all assignments for this course
            var courseAssignments = assignmentRepository.GetAssignmentsByCourse(id);
            foreach (var courseAssignment in courseAssignments)
            {
                //Delete assignment in database
                assignmentRepository.Delete(courseAssignment.ID);
            }

            //Get all enrollments for this course
            var courseEnrollments = enrollmentRepository.GetEnrollmentsByCourse(course.ID);
            foreach (var courseEnrollment in courseEnrollments)
            {
                //Delete enrollments in database
                enrollmentRepository.Delete(courseEnrollment.ID);
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
            List<Enrollment> studEnrollments = enrollmentRepository.GetUserEnrollments(id);
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
