using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public interface IAssignmentRepository
    {
        IEnumerable<Assignment> GetAllAssignments();
        Assignment GetAssignment(int id);
        Assignment Add(Assignment newAssignment);
        Assignment Update(Assignment updatedAssignment);
        Assignment Delete(int id);

        List<Assignment> GetAssignmentsByCourse(int courseId);
        List<Assignment> GetStudentAssignments(int userID, List<Course> courses);
        List<Assignment> GetInstructorAssignments(int userID, List<Course> courses);
    }
}
