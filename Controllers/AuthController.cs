using Microsoft.AspNetCore.Mvc;
using LoginAPI.Models;
using LoginAPI.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _databaseService = new DatabaseService(configuration);
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                Console.WriteLine(request);
                // Validate input
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Username and password are required"
                    });
                }

                // Check user in database
                var user = await _databaseService.ValidateUserAsync(request.Username, request.Password);
                Console.WriteLine(user);
                if (user != null)
                {
                    // Generate JWT token
                    var token = GenerateJwtToken(user.Username);

                    return Ok(new LoginResponse
                    {
                        Success = true,
                        Message = "Login successful",
                        User = user,
                        Token = token
                    });
                }
                else
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // Log the exception
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("test-connection")]
        public async Task<ActionResult> TestConnection()
        {
            var isConnected = await _databaseService.TestConnectionAsync();
            return Ok(new { connected = isConnected });
        }
    }
}