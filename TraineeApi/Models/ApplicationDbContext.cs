using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuestionBankApi.Core.Models;

namespace TraineeApi.Models
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

            // Convert UserRole enum to string for storage in the database
            var userRoleConverter = new ValueConverter<UserRole, string>(
                v => v.ToString(),
                v => (UserRole)Enum.Parse(typeof(UserRole), v));

            // Apply the conversion to the Role property of the User entity
            modelBuilder.Entity<User>()
                .Property(e => e.Role)
                .HasConversion(userRoleConverter);

            // Additional configurations can be added here
        }
    }
}
