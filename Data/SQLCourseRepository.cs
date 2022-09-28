using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLCourseRepository : ICourseRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public SQLCourseRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
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
            if (course != null)
            {
                context.Course.Remove(course);
                context.SaveChanges();
            }
            return course;
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return context.Course;
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
          /*context.Entry(updatedUser).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            var course = context.Course.Find(updatedCourse.ID);
            return course;*/
            return updatedCourse;
         }
    }
}
