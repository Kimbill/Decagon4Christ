using Decagon4Christ.Core.Services.Interface;
using Decagon4Christ.Data.Repositories.Interfaces;
using Decagon4Christ.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Decagon4Christ.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailRepository emailRepository, IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailRepository = emailRepository;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("upload/{filePath}")]
        public async Task<IActionResult> UploadEmails(string filePath)
        {
            try
            {
                await _emailRepository.SaveDataFromExcelToDatabase(filePath);
                return Ok("Emails uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading emails.");
                return StatusCode(500, "An error occurred while uploading emails.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetEmails()
        {
            try
            {
                var emails = await _emailRepository.GetEmails();
                return Ok(emails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching emails.");
                return StatusCode(500, "An error occurred while fetching emails.");
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmails(UserAction userAction, string userEmail)
        {
            try
            {
                await _emailService.SendMail(userAction, userEmail);
                return Ok("Email sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending emails.");
                return StatusCode(500, "An error occurred while sending emails.");
            }
        }

    }
}
