using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tailors_fitv0._2.Models;

namespace Tailors_fitv0._2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AllApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _Configuration;
        private readonly ApplicationDbContext _context;
        public UsersController(UserManager<AllApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _Configuration = configuration;
            _context = context;
        }

        //User Registration
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<ActionResult> RegisterUser(User user)
        {
            var _user = await _userManager.FindByNameAsync(user.username);
            var NewUser = new UserModel();
            NewUser.UserName = user.username;
            NewUser.email = user.email;
            if (_user != null)
            {
                return BadRequest(new 
                {
                    Result = "0",
                    Message = "User Alreaduy Exist! You can login or use other username."
                });
            }
            _user = new AllApplicationUser()
            {
                UserName = user.username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = user.email
            };
            var res = await _userManager.CreateAsync(_user, user.password);
            if (!res.Succeeded) 
            {
                return BadRequest(new
                {
                    Result = "0",
                    Message = "User registration failed at time of creating user!"
                });
             }
            if (! await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (res.Succeeded)
            {
                await _context.users.AddAsync(NewUser);
                await _userManager.AddToRolesAsync(_user, new[] { "User" });
            }
            return Ok(new
            {
                Result = "1",
                Message = "User registered successfully!"
            });
        }
        //Tailor Registration
        [HttpPost]
        [Route("RegisterTailor")]
        public async Task<ActionResult> RegisterTailor(User user)
        {
            var _user = await _userManager.FindByNameAsync(user.username);
            var tailor = new tailorModel();
            tailor.email = user.email;
            tailor.UserName = user.username;
            if (_user != null)
            {
                return BadRequest(new
                {
                    Result = "0",
                    Message = "User Alreaduy Exist! You can login or use other username."
                });
            }
            _user = new AllApplicationUser()
            {
                UserName = user.username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = user.email
            };
            var res = await _userManager.CreateAsync(_user, user.password);
            if (!res.Succeeded)
            {
                return BadRequest(new
                {
                    Result = "0",
                    Message = "User registration failed at time of creating user!"
                });
            }
            if (!await _roleManager.RoleExistsAsync("Tailor"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Tailor"));
            }
            if (res.Succeeded)
            {
                await _context.tailors.AddAsync(tailor);
                await _userManager.AddToRolesAsync(_user, new[] { "Tailor" });
            }
            return Ok(new
            {
                Result = "1",
                Message = "User registered successfully!"
            });
        }
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            var _user = await _userManager.FindByNameAsync(model.username);
            if (_user != null && await _userManager.CheckPasswordAsync(_user, model.password))
            {
                var UserRoles = await _userManager.GetRolesAsync(_user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,_user.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach(var role in UserRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role,role));
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _Configuration["JWT:ValidIssuer"],
                    audience: _Configuration["JWT:ValidAudience"],
                    expires:DateTime.Now.AddHours(1),
                    claims:authClaims,
                    signingCredentials:new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256)
                    );
                var MainRole = "";
                if (UserRoles.Contains("User")) MainRole = "User";
                else if (UserRoles.Contains("Tailor")) MainRole = "Tailor";
                else MainRole = "Admin";
                return Ok(new
                {
                    Result="1",
                    Message="Login Successfull",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    Role=MainRole
                });
            }
            return Unauthorized(new
            {
                Result="0",
                Message="User Credentials are not registered!"
            });
        }
    }
}
