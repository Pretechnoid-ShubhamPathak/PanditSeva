using Data.Models;
using Data.DTOs;
using Data.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PanditSeva.Identity;
using PanditSeva.Identity.IdentityServices;
using NuGet.Common;

namespace PanditSeva.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly JwtService _jwtService;
        private readonly EncryptedJwtService _encryptedJwtService;

        public AuthController(AppDbContext context, PasswordService passwordService, JwtService jwtService, EncryptedJwtService encryptedJwtService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _encryptedJwtService = encryptedJwtService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new ApplicationUser
            {
                Name = dto.Name!,
                Email = dto.Email,
                Phone = dto.Phone!,
                Role = dto.Role
            };

            user.PasswordHash = _passwordService.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
            var token = _encryptedJwtService.GenerateEncryptedToken(user);

            return Ok(new { SuccessMsg = "Registration successful", Token = token });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var isPasswordValid = _passwordService.VerifyPassword(user, dto.Password, user.PasswordHash!);
            if (!isPasswordValid)
                return Unauthorized("Invalid credentials");

            //var token = _jwtService.GenerateToken(user);
            var token = _encryptedJwtService.GenerateEncryptedToken(user);

            return Ok(new { SuccessMsg = "Login successful", Token = token });
        }
    }
}



