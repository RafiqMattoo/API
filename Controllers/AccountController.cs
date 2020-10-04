using System.Text;
using API.Entities;
using System;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using API.DTO;
using System.Collections.Generic;
using System.Linq;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
    
{
    [Authorize]
    public class AccountController : BaseApiController
    {
        public DataContext datacontext { get; set; }
        public ITokenService TokenService { get; }

        public AccountController(DataContext _datacontext,ITokenService tokenService)
        {
            this.datacontext = _datacontext;
            TokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO userDTO)
        {

            if (await UserExists(userDTO.Username))
            {
                return BadRequest("Username Already Exists in the System");
            }
            else
            {
                using var hmac = new HMACSHA512();
                var user = new AppUser
                {
                    Username = userDTO.Username,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDTO.Password)),
                    PasswordSalt = hmac.Key
                };
                datacontext.Users.Add(user);
                await datacontext.SaveChangesAsync();
                return new UserDTO
                {
                    Username = userDTO.Username,
                    Token = TokenService.CreateToken(user)
                };
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user =  await datacontext.Users.FirstOrDefaultAsync(x => x.Username == loginDTO.Username);
            if (user == null)
            {
                return Unauthorized("Invalid Username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computehash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for (int i = 0; i < computehash.Length; i++)
            {
                if (computehash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }
            return new UserDTO
            {
                Username = loginDTO.Username,
                Token = TokenService.CreateToken(user)
            };


        }
        public async Task<bool> UserExists(string username)
        {
            return await datacontext.Users.AnyAsync(x => x.Username == username);


        }


    }
}