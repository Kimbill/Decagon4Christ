using Decagon4Christ.Common.Securities;
using Decagon4Christ.Core.Services.Implementation;
using Decagon4Christ.Data;
using Decagon4Christ.Model;
using Decagon4Christ.Model.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Decagon4Christ.Controllers
{
    //[ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly Utilities _utilities;

        public UserController(IConfiguration configuration, EmailService emailService, AppDbContext dbContext, UserManager<User> userManager, ILogger<UserController> logger, Utilities utilities)
        {
            _configuration = configuration;
            _emailService = emailService;
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _utilities = utilities;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Comments = model.Comments,
                    PasswordHash = model.Password,
                    TechStack = model.TechStack,
                    Squad = model.Squad,
                    PlaceOfWork = model.PlaceOfWork,
                    Role = model.Role,
                    PhoneNumber = model.PhoneNumber,
                    
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Handle email confirmation if required

                    return Ok("Registration successful");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, "An error occurred during registration.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = _utilities.GenerateJwt(user); // Call the GenerateJwt method through the instance of the Utilities class

                    return Ok(new { Token = token });
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "An error occurred during login.");
            }
        }


    }
}
