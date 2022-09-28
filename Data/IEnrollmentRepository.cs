using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public interface IEnrollmentRepository
    {
        IEnumerable<Enrollment> GetAllEnrollments();
        Enrollment GetEnrollment(int id);
        Enrollment Add(Enrollment newEnrollment);
        Enrollment Update(Enrollment updatedEnrollment);
        Enrollment Delete(int id);
    }
}
