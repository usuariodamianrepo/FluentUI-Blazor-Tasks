using AutoMapper;
using BackEnd.API.Data;
using BackEnd.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class UserAccountsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOptions<JwtSection> _config;

        public UserAccountsController(AppDbContext context, IMapper mapper, IOptions<JwtSection> config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<GeneralResponse>> CreateAsync([FromBody] UserRegisterDTO user)
        {
            if (user == null) return BadRequest("Model is empty");

            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null) return new GeneralResponse(false, "User registeres already");

            var applUser = await _context.AppUsers.AddAsync(new AppUser()
            {
                FullName = user.Fullname,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.ConfirmPassword),
                LockoutEnabled = false
            });

            try
            {
                var newUserId = await _context.SaveChangesAsync();

                var appRole = await _context.AppRoles.FirstAsync();
                if (appRole == null) return new GeneralResponse(false, "There is not Role to assigmnet.");

                var appUserRole = new AppUserRole
                {
                    UserId = newUserId,
                    RoleId = appRole.Id
                };
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new GeneralResponse(false, $"An error occurred while saving the user: {ex.Message}"));
            }

            return Ok(new GeneralResponse(true, $"New User was created {user.Email}"));
        }

        private async Task<AppUser> FindUserByEmail(string email) => await _context.AppUsers.FirstOrDefaultAsync(u => u.Email.Equals(email));


        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> SignInAsync([FromBody] UserLoginDTO user)
        {
            if (user is null) return new LoginResponse(false, "Model is empty");

            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null) return new LoginResponse(false, "User not found");

            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
                return new LoginResponse(false, "Email or/and password not valid");

            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null) return new LoginResponse(false, "User role not found");

            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName is null) return new LoginResponse(false, "User role not found");

            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name!);
            string refreshToken = GenerateResfreshToken();

            var findUser = await _context.RefreshTokenInfos.FirstOrDefaultAsync(x =>
                x.UserId == applicationUser.Id);

            if (findUser is not null)
            {
                findUser!.Token = refreshToken;
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.RefreshTokenInfos.AddAsync(new RefreshTokenInfo()
                {
                    Token = refreshToken,
                    UserId = applicationUser.Id
                });
                await _context.SaveChangesAsync();
            }

            return new LoginResponse(true, "Login succesfull", jwtToken, refreshToken);
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponse>> RefreshTokenAsync([FromBody] RefreshTokenDTO token)
        {
            if (token is null) return new LoginResponse(false, "Model is empty");

            var findToken = await _context.RefreshTokenInfos.FirstOrDefaultAsync(x =>
                x.Token!.Equals(token.Token));

            if (findToken is null) return new LoginResponse(false, "Refresh token is required");

            var user = await _context.AppUsers.FirstOrDefaultAsync(x =>
                x.Id == findToken.UserId);

            if (user is null)
                return new LoginResponse(false, "Refresh token could not be generated because user not found");

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.RoleId);
            string jwtToken = GenerateToken(user, roleName.Name!);
            string refreshToken = GenerateResfreshToken();

            var updateRefreshToken = await _context.RefreshTokenInfos.FirstOrDefaultAsync(x =>
                x.UserId == user.Id);
            if (updateRefreshToken is null)
                return new LoginResponse(false, "Refresh token could not be generated because user has not signin");

            updateRefreshToken.Token = refreshToken;
            await _context.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed sucessful", jwtToken, refreshToken);
        }

        /*
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync()
        {
           var users = await accountInterface.GetUsers();
           if (users == null) return NotFound();
           return Ok(users);
        }

        //[HttpPut("update-user")]
        //public async Task<IActionResult> UpdateUser([FromBody] ManageUser manageUser)
        //{
        //    var result = await accountInterface.UpdateUser(manageUser);
        //    return Ok(result);
        //}

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
           var users = await accountInterface.GetRoles();
           if (users == null) return NotFound();
           return Ok(users);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
           var result = await accountInterface.DeleteUser(id);
           return Ok(result);
        }
        */

        private string GenerateToken(AppUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!)
            };

            var token = new JwtSecurityToken(
                issuer: _config.Value.Issuer,
                audience: _config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateResfreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<AppUserRole> FindUserRole(int userId) =>
            await _context.AppUserRoles.FirstOrDefaultAsync(x => x.UserId == userId);

        private async Task<AppRole> FindRoleName(int roleId) =>
            await _context.AppRoles.FirstOrDefaultAsync(x => x.Id == roleId);

        private async Task<List<AppRole>> SystemRoles() => await _context.AppRoles
            .AsNoTracking()
            .ToListAsync();

        private async Task<List<AppUserRole>> UserRoles() => await _context.AppUserRoles
            .AsNoTracking()
            .ToListAsync();

        private async Task<List<AppUser>> GetApplicationUsers() => await _context.AppUsers
            .AsNoTracking()
            .ToListAsync();
    }
}
