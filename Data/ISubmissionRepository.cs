using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public interface ISubmissionRepository
    {
        IEnumerable<Submission> GetAllSubmissions();
        Submission GetSubmission(int id);
        Submission Add(Submission newSubmission);
        Submission Update(Submission updatedSubmission);
        Submission Delete(int id);
        IEnumerable<Submission> GetSubmissionsByAssignment(int assignmentId);

        List<Submission> GetSubmissionsByAssignmentUserList(int assignmentId, int userId);
    }
}
