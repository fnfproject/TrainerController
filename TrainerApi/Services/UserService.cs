using Microsoft.EntityFrameworkCore;
using QuestionBankApi.Core.Dtos;
using QuestionBankApi.Core.Models;
using QuestionBankApi.Trainer.Interfaces;
using QuestionBankApi.Trainer.Services;
using TrainerApi.Models;

namespace TrainerApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly TwoFactorAuthService _twoFactorAuthService;
        private readonly JwtService _jwtService;

        public UserService(ApplicationDbContext context, TwoFactorAuthService twoFactorAuthService, JwtService jwtService)
        {
            _context = context;
            _twoFactorAuthService = twoFactorAuthService;
            _jwtService = jwtService;
        }

        public User RegisterUser(RegisterDto model, UserRole role)
        {
            var existingUser = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == model.Username || u.Email == model.Email);

            if (existingUser != null)
            {
                return null; // Indicates username or email is already taken
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Is2FAEnabled = false,
                IsVerified = false,
                IsAdminApproved = false, // Default value
                Role = role // Set the role here
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }


        public User AuthenticateUser(string username, string password)
        {
            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null; // Authentication failed
            }

            // Check if 2FA is enabled and not verified
            if (user.Is2FAEnabled && !user.IsVerified)
            {
                return null; // 2FA verification required
            }

            return user;
        }

        public (byte[] QrCode, string SecretKey) EnableTwoFactorAuth(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return (null, null); // User not found

            var secretKey = _twoFactorAuthService.GenerateSecretKey();
            var qrCodeUri = _twoFactorAuthService.GenerateQrCodeUri(user.Email, secretKey, "QuizApp");

            user.TwoFactorSecretKey = secretKey;
            user.Is2FAEnabled = true;
            user.IsVerified = false; // Reset 2FA verification status when enabling 2FA
            _context.SaveChanges();

            var qrCode = _twoFactorAuthService.GenerateQrCode(qrCodeUri);
            return (qrCode, secretKey);
        }

        public bool VerifyTwoFactorCode(string username, string code)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || !user.Is2FAEnabled) return false; // User not found or 2FA not enabled

            return _twoFactorAuthService.ValidateTwoFactorCode(user.TwoFactorSecretKey, code);
        }

        public User MarkUserAsVerified(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;

            user.IsVerified = true;
            _context.SaveChanges();

            return user;

        }

        public void ApprovedTrainer(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) throw new ArgumentException("User not found.");

            user.IsAdminApproved = true;
            _context.SaveChanges();
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

        // Method to add a new user
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}

