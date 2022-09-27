using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CS3750_PlanetExpressLMS.Models;

namespace CS3750_PlanetExpressLMS.Data
{
    public class CS3750_PlanetExpressLMSContext : DbContext
    {
        public CS3750_PlanetExpressLMSContext (DbContextOptions<CS3750_PlanetExpressLMSContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
    }
}
