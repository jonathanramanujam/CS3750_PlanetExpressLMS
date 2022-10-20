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

        IEnumerable<Assignment> GetAssignmentsByCourse(int courseId);
        IEnumerable<Assignment> GetStudentAssignments(int userID);
        IEnumerable<Assignment> GetInstructorAssignments(int userID);
    }
}
