














using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionBankApi.Core.Models
{
    [Table("UserTbl")]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? TwoFactorSecretKey { get; set; }
        public bool Is2FAEnabled { get; set; }
        public bool IsVerified { get; set; }
        public bool IsAdminApproved { get; set; }

        // Directly map the enum with EF value conversion
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

        // Method to get CreatedAt in the desired format
        public string GetFormattedCreatedAt()
        {
            return CreatedAt.ToString("M/d/yyyy h:mm:ss tt");
        }
    }
}

