using Microsoft.EntityFrameworkCore;
using QuestionBankApi.Core.Models;

namespace TrainerApi.Models
{
    
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
            {
            }

            public DbSet<User> Users { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Additional configurations can be added here
            }
        }
    
}
