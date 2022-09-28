using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLEnrollmentRepository : IEnrollmentRepository
    {
        public Enrollment Add(Enrollment newEnrollment)
        {
            return newEnrollment;
        }

        public Enrollment Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Enrollment> GetAllEnrollments()
        {
            throw new System.NotImplementedException();
        }

        public Enrollment GetEnrollment(int id)
        {
            throw new System.NotImplementedException();
        }

        public Enrollment Update(Enrollment updatedEnrollment)
        {
            throw new System.NotImplementedException();
        }
    }
}
