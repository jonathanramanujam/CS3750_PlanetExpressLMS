using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public interface ICourseRepository
    {
        IEnumerable<Course> GetAllCourses();
        List<Course> GetUserCourses(int id);
        List<Course> GetStudentCourses(int id);
        Course GetCourse(int id);
        Course Add(Course newCourse);
        Course Delete(int id);
        Course Update(Course updatedCourse);
    }
}
