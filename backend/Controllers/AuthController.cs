﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Interfaces;
using backend.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;

        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser([FromQuery] Guid token)
        {
            try
            {
                UserViewModel user = await _authService.GetCurrentUser(token);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchForUsers([FromQuery] string searchString)
        {
            try
            {
                List<UserViewModel> users = await _authService.SearchForUsers(searchString);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockedUsers()
        {
            try
            {
                List<UserViewModel> users = await _authService.GetBlockedUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                List<UserViewModel> users = await _authService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
        {
            try
            {
                UserViewModel user = await _authService.Login(email, password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Logout([FromQuery] Guid token)
        {
            try
            {
                await _authService.Logout(token);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserViewModel userData, [FromQuery] string emailActivationUrl, [FromQuery] string subject, [FromQuery] string mailContent)
        {
            try
            {
                await _authService.Register(userData, emailActivationUrl, subject, mailContent);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActivateAccount([FromQuery] Guid token)
        {
            try
            {
                await _authService.ActivateAccount(token);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> DeleteAccount([FromQuery] Guid token, [FromQuery] string password)
        {
            try
            {
                await _authService.SetAccountUnactive(token, password);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeData passwordChangeData, [FromQuery] Guid token)
        {
            try
            {
                await _authService.ChangePassword(token, passwordChangeData);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> RemindPassword([FromQuery] string email, [FromQuery] string subject, [FromQuery] string mailContent)
        {
            try
            {
                await _authService.RemindPassword(email, subject, mailContent);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> DisableUsersSystemAccess([FromQuery] int[] ids)
        {
            try
            {
                await _authService.DisableUsersSystemAccess(ids);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> DisableUsersSystemEditing([FromQuery] int[] ids)
        {
            try
            {
                await _authService.DisableUsersSystemEditing(ids);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UnlockUser([FromQuery] int id)
        {
            try
            {
                await _authService.UnlockUser(id);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}