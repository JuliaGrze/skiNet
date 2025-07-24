using API.DTO;
using API.Extensions;
using CORE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        //Register
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDTO registerDTO)
        {
            var user = new AppUser
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

            return Ok();
        }

        //Log out
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        //User information
        //wyjatkowo nie dajemy authorize bo w przegladarce nie mamy dostepu do cookie, wiec jesli hcemy sprawdzic czy uzytkownik jest zalogowany to to nam pomoze
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if(User.Identity?.IsAuthenticated == false) return NoContent();

            var user = await _userManager.GetUserByEmailWithAddress(User);

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                Address = user.Address?.ToDto(),
                Roles = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        //Sprawdza, czy obecne żądanie pochodzi od zalogowanego użytkownika
        [HttpGet("auth-status")]
        public ActionResult GetAuthState()
        {
            return Ok( new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false});
        }

        [Authorize]
        [HttpPost("address")]
        public async Task<ActionResult<Address>> CreateOrUpdateAddress(AddressDto addressDto)
        {
            var user = await _userManager.GetUserByEmailWithAddress(User);

            if(user.Address == null)
            {
                //Create new Address
                user.Address = addressDto.ToEntity();
            }
            else
            {
                //Update existing Address
                user.Address.UpdateFromDto(addressDto);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Probelm updating user address");

            return Ok(user.Address.ToDto());
        }

    }
}
