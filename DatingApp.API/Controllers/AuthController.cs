using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(/*IAuthRepository repo*/IConfiguration config, IMapper mapper,
                             UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._config = config;
            this._mapper = mapper;
            // this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            #region This is taken care of by identity and other commented code in this method
            //  userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            // if (await this._repo.UserExists(userForRegisterDto.UserName))
            //     return BadRequest("Username already exists");
            #endregion
           
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate,userForRegisterDto.Password);

            // var createdUser = await this._repo.Register(userToCreate, userForRegisterDto.Password);
            

            if(result.Succeeded) {
                 var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
                 return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);

            // return CreatedAtRoute("GetUser", new { controller = "Users", id = userToReturn.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user  = await _userManager.FindByNameAsync(userForLoginDto.Username);

            if(user == null)
               return BadRequest("Failed to find Username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password,false);

            if(result.Succeeded)
            {
                var appUser = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());
                
                var userToReturn = _mapper.Map<UserForListDto>(appUser);

               // var meraToken = await GenerateJwtToken(appUser);
                 return Ok(new
            {
                token = GenerateJwtToken(appUser).Result,
                user = userToReturn
            });
                
            }

            return Unauthorized();
           
            // var userFromRepo = await this._repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            // if (userFromRepo == null)
            //     return Unauthorized();



            // var user = _mapper.Map<UserForListDto>(userFromRepo);

           


            // return Ok(new
            // {
            //     token = tokenHandler.WriteToken(token),
            //     user
            // });

        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)

            };

            var roles =  await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
                
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                            .GetBytes(this._config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}