using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLSubmissionRepository :ISubmissionRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public SQLSubmissionRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }

        public Submission Add(Submission newSubmission)
        {
            context.Submission.Add(newSubmission);
            context.SaveChanges();
            return newSubmission;
        }

        public Submission Delete(int id)
        {
            Submission sub = context.Submission.Find(id);
            if(sub != null)
            {
                context.Submission.Remove(sub);
                context.SaveChanges();
            }
            return sub;
        }

        public IEnumerable<Submission> GetAllSubmissions()
        {
            return context.Submission;
        }

        public Submission GetSubmission(int id)
        {
            return context.Submission.Find(id);
        }

        public IEnumerable<Submission> GetSubmissionsByAssignment(int assignmentId)
        {
            var subs = GetAllSubmissions();
            subs = subs.Where(s => s.AssignmentID == assignmentId);
            return subs;
        }

        public Submission Update(Submission updatedSubmission)
        {
            //For notes on updating part of a class, see any other SQLRepository.
            var sub = context.Submission.Attach(updatedSubmission);
            sub.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return updatedSubmission;
        }
    }
}
