using Bogus;
using Microsoft.EntityFrameworkCore;
using Server.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    public class ServerDbContext : DbContext
    {
        public DbSet<DbPerson> People { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=gRPCDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            int id = 1;
            var faker = new Faker<DbPerson>()
                .RuleFor(p => p.Fname, opt => opt.Person.FirstName)
                .RuleFor(p => p.Lname, opt => opt.Person.LastName)
                .RuleFor(p => p.Id, _ => id++);
            modelBuilder.Entity<DbPerson>()
                .HasData(faker.Generate(100));
            base.OnModelCreating(modelBuilder);
        }
    }
}
