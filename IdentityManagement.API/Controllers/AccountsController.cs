using IdentityManagement.API.Model.Entities;
using IdentityManagement.API.Model.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Data;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        // GET api/<AccountsController>/5
        [HttpGet("all-users"), Authorize(Roles = "Admin")]
        public List<AppUser> GetAllUser()
        {
            return _userManager.Users.ToList();
        }

        [HttpGet("users"), Authorize(Roles = "User,Admin")]
        public async Task<List<AppUser>> GetUser()
        {
            List<AppUser> usersWithUserRole = new();
            foreach (var user in _userManager.Users)
            {
                if (!await _userManager.IsInRoleAsync(user, "User"))
                    usersWithUserRole.Add(user);
            }
            return usersWithUserRole;
        }
        // POST api/<AccountsController>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userFound = await _userManager.FindByEmailAsync(request.Email);
            if (userFound != null)
            {
                var result = await _signInManager.PasswordSignInAsync(userFound, request.Password, true, false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(userFound); var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role,string.Join(',',roles))                       
                    };                     
                await _signInManager.SignInWithClaimsAsync(userFound, true, claims);

                return Ok("User logged in Successfuly.");
            }
            else
                return BadRequest("Password is not correct.");
        }
            else
                return NotFound($"User with email '{request.Email}' not exists.");

    }

    [HttpPost("register-user")]
    public async Task<ActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        string successMessage = string.Empty;
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userFound = await _userManager.FindByEmailAsync(request.Email);
        if (userFound != null)
            return BadRequest($"User email '{request.Email}' is already taken. Please try another.");
        AppUser user = new()
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
        };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
            successMessage = "User registered Successfuly.";
        else
            return BadRequest(result.Errors);

        var role = _roleManager.Roles.FirstOrDefault(x => x.Name.Equals("User"));
        if (role != null)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!roleResult.Succeeded)
                successMessage += "\n" + "User added in User Role successfuly.";
            else
                return BadRequest(roleResult.Errors);
        }

        return Ok(successMessage);
    }

    [HttpPost("register-Admin")]
    public async Task<ActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        string successMessage = string.Empty;
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userFound = await _userManager.FindByEmailAsync(request.Email);
        if (userFound != null)
            return BadRequest($"User email '{request.Email}' is already taken. Please try another.");
        AppUser user = new()
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
        };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
            successMessage = "User registered Successfuly.";
        else
            return BadRequest(result.Errors);

        var role = _roleManager.Roles.FirstOrDefault(x => x.Name.Equals("Admin"));
        if (role != null)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!roleResult.Succeeded)
                successMessage += "\n" + "User added in User Role successfuly.";
            else
                return BadRequest(roleResult.Errors);
        }

        return Ok(successMessage);
    }

}
}
