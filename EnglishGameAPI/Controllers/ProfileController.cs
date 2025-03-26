using EnglishGameAPI.Models;
using EnglishGameAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EnglishGameAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileDataService _profileDataService;

        public ProfileController(IProfileDataService profileDataService)
        {
            _profileDataService = profileDataService;
        }

        [HttpGet("Get")]
        public IActionResult Get(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return BadRequest("Username is required");
                }

                var profile = _profileDataService.GetProfile(username);
                if (profile == null)
                {
                    return NotFound($"Profile for username '{username}' not found");
                }

                // Don't return password hash to client
                var response = new
                {
                    profile.Username,
                    profile.Email,
                    profile.LastUpdated
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody] Profile profile)
        {
            try
            {
                if (profile == null || string.IsNullOrWhiteSpace(profile.Username))
                {
                    return BadRequest("Invalid profile data");
                }

                // In a real app, you'd hash the password here
                profile.PasswordHash = profile.PasswordHash; // Just storing as-is for this example

                _profileDataService.AddProfile(profile);

                return Ok(new { message = "Profile added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] Profile profile)
        {
            try
            {
                if (profile == null || string.IsNullOrWhiteSpace(profile.Username))
                {
                    return BadRequest("Invalid profile data");
                }

                // In a real app, you'd hash the password here if it was changed
                _profileDataService.UpdateProfile(profile);

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}