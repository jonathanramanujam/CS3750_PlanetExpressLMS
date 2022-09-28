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

        public DbSet<User> User { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Enrollment> Enrollment { get; set; }
    }
}
