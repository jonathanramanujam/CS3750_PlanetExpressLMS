using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CS3750_A1.Models;

namespace CS3750_A1.Data
{
    public class CS3750_A1Context : DbContext
    {
        public CS3750_A1Context (DbContextOptions<CS3750_A1Context> options)
            : base(options)
        {
        }

        public DbSet<Credential> Credential { get; set; }
    }
}
