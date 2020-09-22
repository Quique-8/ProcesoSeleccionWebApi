using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PruebaProsegur.WebApi.Data;
using PruebaProsegur.WebApi.Data.Entities;
using PruebaProsegur.WebApi.Models;

namespace PruebaProsegur.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(DataContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel userLogin)
        {
            var _userInfo = await AutenticarUsuarioAsync(userLogin.Username, userLogin.Password);
            if (_userInfo != null)
            {
                return Ok(new { token = CreateTokenJWT(_userInfo) });
            }
            else
            {
                return Unauthorized();
            }
        }

        // COMPROBAMOS SI EL USUARIO EXISTE EN LA BASE DE DATOS 
        private async Task<User> AutenticarUsuarioAsync(string userName, string password)
        {
            // AQUÍ LA LÓGICA DE AUTENTICACIÓN //
            //var user = await _userManager.FindByEmailAsync(userName); // Incapaz de hacerlo por este método.

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(userName.ToLower()) && x.Password.Equals(password));

            if (user != null)
            {
                return new User()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
            }

            return null;
        }

        // GENERAMOS EL TOKEN CON LA INFORMACIÓN DEL USUARIO
        private string CreateTokenJWT(User usuarioInfo)
        {
            // CREAMOS EL HEADER
            var _symmetricSecurityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])
                );
            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _Header = new JwtHeader(_signingCredentials);

            // CREAMOS LOS CLAIMS
            var _Claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, usuarioInfo.Id.ToString()),
                new Claim("nombre", usuarioInfo.FirstName),
                new Claim("apellidos", usuarioInfo.LastName),
                new Claim(JwtRegisteredClaimNames.Email, usuarioInfo.Email)
            };

            // CREAMOS EL PAYLOAD
            var _Payload = new JwtPayload(
                    issuer: _configuration["Tokens:Issuer"],
                    audience: _configuration["Tokens:Audience"],
                    claims: _Claims,
                    notBefore: DateTime.UtcNow,
                    // Exipra a la 24 horas.
                    expires: DateTime.UtcNow.AddHours(24)
                );

            // GENERAMOS EL TOKEN
            var _Token = new JwtSecurityToken(
                    _Header,
                    _Payload
                );

            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }
    }
}
