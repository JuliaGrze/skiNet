using CORE.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        //Wyszukuje użytkownika w bazie danych (tabeli użytkowników Identit)
        public static async Task<AppUser> GetUserByEmail(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users.FirstOrDefaultAsync(x => 
                x.Email == user.GetEmail());

            return userToReturn == null 
                ? throw new AuthenticationException("User not found") 
                : userToReturn;
        }

        //allows you to find the currently logged in user in the database along with their address
        public static async Task<AppUser> GetUserByEmailWithAddress(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Email == user.GetEmail());

            return userToReturn == null
                ? throw new AuthenticationException("User not found")
                : userToReturn;
        }

        //wyciąga e-mail z claimów zalogowanego użytkownika (z tokena/cookie).
        public static string GetEmail(this ClaimsPrincipal user)
        {
            var email =  user.FindFirstValue(ClaimTypes.Email);

            return email == null 
                ? throw new AuthenticationException("Email claim not found") 
                : email;
        }
    }
}
