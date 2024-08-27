using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestionBankApi.Core.Dtos;
using QuestionBankApi.Trainee.Interfaces ;
using QuestionBankApi.Core.Models;
using TraineeApi.Services;
using QuestionBankApi.Trainee.Dtos;

namespace TraineeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("cors")]
    public class TraineeController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;

        public TraineeController(JwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userService.RegisterUser(model, UserRole.Trainee);
            if (user == null)
                return Conflict("Username or Email already taken.");

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _userService.AuthenticateUser(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user.Username, user.Is2FAEnabled);

            // Return the response as LoginResponseDto
            var responseDto = new LoginResponseDto
            {
                Token = token,
                Id = user.Id  // Assuming 'ID' is the correct property name in your 'user' model
            };

            return Ok(responseDto);
        }



        [HttpPost("enable-2fa")]
        public IActionResult Enable2FA([FromBody] Enable2FADto model)
        {
            var (qrCode, _) = _userService.EnableTwoFactorAuth(model.Username);
            if (qrCode == null) return NotFound("User not found.");

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

            // Update the user's verification status in the UserService
            var user = _userService.MarkUserAsVerified(model.Username);

            // Generate a JWT token after successful verification
            var token = _jwtService.GenerateToken(user.Username, user.Is2FAEnabled);

            // Return a success message along with the token
            return Ok(new
            {
                Message = "2FA verification successful.",
                Token = token
            });
        }
    }
}
