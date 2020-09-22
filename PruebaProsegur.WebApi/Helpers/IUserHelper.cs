using Microsoft.AspNetCore.Identity;
using PruebaProsegur.WebApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaProsegur.WebApi.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<SignInResult> ValidatePasswordAsync(User user, string password);
    }
}
