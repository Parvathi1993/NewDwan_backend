using Microsoft.AspNetCore.Mvc;
using LoginAPI.Models;
using LoginAPI.Services;

namespace LoginAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public AuthController(IConfiguration configuration)
        {
            _databaseService = new DatabaseService(configuration);
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
                    return Ok(new LoginResponse
                    {
                        Success = true,
                        Message = "Login successful",
                        User = user
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
            catch (Exception)
            {
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }

        [HttpGet("test-connection")]
        public async Task<ActionResult> TestConnection()
        {
            var isConnected = await _databaseService.TestConnectionAsync();
            return Ok(new { connected = isConnected });
        }

        // [HttpPost("getData")]
        // public async Task<ActionResult<GetDataResponse>> GetData([FromBody] GetDataResponse request)
        // {
        //     try
        //     {
        //         Console.WriteLine(request);
        //         // Validate input
        //         if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        //         {
        //             return BadRequest(new GetDataResponse
        //             {
        //                 Success = false,
        //                 Message = "Username and password are required"
        //             });
        //         }

        //         // Check user in database
        //         var user = await _databaseService.ValidateUserAsync(request.Username, request.Password);
        //         Console.WriteLine(user);
        //         if (user != null)
        //         {
        //             return Ok(new GetDataResponse
        //             {
        //                 Success = true,
        //                 Message = "Login successful",
        //                 User = user
        //             });
        //         }
        //         else
        //         {
        //             return Unauthorized(new GetDataResponse
        //             {
        //                 Success = false,
        //                 Message = "Invalid username or password"
        //             });
        //         }
        //     }
        //     catch (Exception)
        //     {
        //         return StatusCode(500, new GetDataResponse
        //         {
        //             Success = false,
        //             Message = "Internal server error"
        //         });
        //     }
    }

}
