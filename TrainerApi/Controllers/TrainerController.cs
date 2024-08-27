using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionBankApi.Core.Dtos;
using QuestionBankApi.Core.Models;
using QuestionBankApi.Trainer.Dtos;
using QuestionBankApi.Trainer.Interfaces;
using QuestionBankApi.Trainer.Services;
using TrainerApi.Services;



namespace TrainerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;
        private const string HardcodedAdminVerificationCode = "TR123456"; // Hardcoded admin verification code

        public TrainerController(JwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Register the trainer with 'Trainer' role and default 'IsAdminApproved' as false
            var user = _userService.RegisterUser(model, UserRole.Trainer);

            if (user == null)
                return Conflict("Username or Email already taken.");

            // Notify admin about new trainer registration
            // _emailService.NotifyAdminAboutRegistration(user.Email);

            return Ok("Trainer registered successfully. Admin approval required.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _userService.AuthenticateUser(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Check if the user has enabled 2FA
            if (!user.Is2FAEnabled)
            {
                if (!user.IsVerified)
                {
                    return Unauthorized("User is not verified. Please complete 2FA verification.");
                }
            }

            if (!user.IsAdminApproved)
            {
                return Unauthorized("Trainer not approved by admin.");
            }

            var token = _jwtService.GenerateToken(user.Username, user.Is2FAEnabled);
            return Ok(new LoginResponseDto { Token = token, Id = user.Id });
        }

        [HttpPost("enable-2fa")]
        public IActionResult Enable2FA([FromBody] Enable2FADto model)
        {
            var (qrCode, _) = _userService.EnableTwoFactorAuth(model.Username);
            if (qrCode == null) return NotFound("Trainer not found.");

            return File(qrCode, "image/png");
        }

        [HttpPost("verify-2fa")]
        public IActionResult Verify2FA([FromBody] Verify2FADto model)
        {
            // Verify the 2FA code
            var isValid = _userService.VerifyTwoFactorCode(model.Username, model.Code);

            if (!isValid)
            {
                return Unauthorized("Invalid 2FA code.");
            }

            // Update the trainer's verification status in the UserService
            var user = _userService.MarkUserAsVerified(model.Username);

            // Return a message indicating 2FA verification is successful
            return Ok(new { Message = "2FA verification successful. Enter the admin verification code to complete the process." });
        }

        [HttpPost("verify-admin-code")]
        public IActionResult VerifyAdminCode([FromBody] VerifyAdminCodeDto model)
        {
            if (model.Code != HardcodedAdminVerificationCode)
            {
                return Unauthorized("Invalid admin verification code.");
            }

            return Ok("Admin verification successful. You can now login.");
        }
    }
}
