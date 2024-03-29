﻿using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public interface IEnrollmentRepository
    {
        IEnumerable<Enrollment> GetAllEnrollments();

        List<Enrollment> GetUserEnrollments(int userId);
        List<Enrollment> GetEnrollmentsByCourse(int courseID);
        Enrollment GetEnrollment(int id);
        Enrollment Add(Enrollment newEnrollment);
        Enrollment Update(Enrollment updatedEnrollment);
        Enrollment Delete(int id);
        List<Enrollment> GetStudentsEnrolled(int courseID);
    }
}
