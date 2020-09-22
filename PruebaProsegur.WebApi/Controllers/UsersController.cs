using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaProsegur.Models;
using PruebaProsegur.WebApi.Data;
using PruebaProsegur.WebApi.Data.Entities;

namespace PruebaProsegur.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()  // Para pruebas rapidas durante el desarrollo.
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUser(EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _context.Users
                .Include(o => o.Explications)
                .FirstOrDefaultAsync(o => o.Email.ToLower() == emailRequest.Email.ToLower());

            if (user == null)
            {
                return NotFound();
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Explications = user.Explications.Select(e => new ExplicationResponse
                {
                    Id = e.Id,
                    Description = e.Description
                }).ToList()
            };

            return Ok(response);
        }
    }
}
