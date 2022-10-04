﻿using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLAssignmentRepository : IAssignmentRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public SQLAssignmentRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }

        public Assignment Add(Assignment newAssignment)
        {
            context.Assignment.Add(newAssignment);
            context.SaveChanges();
            return newAssignment;
        }

        public Assignment Delete(int id)
        {
            Assignment assignment = context.Assignment.Find(id);
            if(assignment != null)
            {
                context.Assignment.Remove(assignment);
                context.SaveChanges();
            }
            return assignment;
        }

        public IEnumerable<Assignment> GetAllAssignments()
        {
            return context.Assignment;
        }

        public Assignment GetAssignment(int id)
        {
            return context.Assignment.Find(id);
        }

        public Assignment Update(Assignment updatedAssignment)
        {
            var ass = context.Assignment.Attach(updatedAssignment);
            //Right now, this updates the ENTIRE Assignment object.
            //If you don't want that to happen, remove the below statement:
            ass.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            //Then add statements here like, for example:
            //context.Entry(updatedAssignment).Property("CourseID").IsModified = true;
            //And ONLY those attributes will be updated.
            context.SaveChanges();
            //Then, replace the return statement with this to 'reset' the context and get good values:
            /*context.Entry(updatedAssignment).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
              var assignment = context.Enrollment.Find(updatedAssignment.ID);
              return assignment;*/
            return updatedAssignment;
        }
    }
}
