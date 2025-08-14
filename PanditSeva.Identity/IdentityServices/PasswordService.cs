using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Identity;

namespace PanditSeva.Identity
{
    public class PasswordService
    {
        private readonly PasswordHasher<ApplicationUser> _passwordHasher;

        public PasswordService()
        {
            _passwordHasher = new PasswordHasher<ApplicationUser>();
        }

        public string HashPassword(ApplicationUser user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(ApplicationUser user, string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }

}
