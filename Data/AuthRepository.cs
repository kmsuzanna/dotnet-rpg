using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _config = configuration;
            _context = context;
            
        }
        public async Task<ServiceResponse<string>> Login(string userName, string password)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Username.ToLower().Equals(userName.ToLower()));
            if (user is null){
                response.Success=false;
                response.Message = "User Not Found";
            }

            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                response.Success=false;
                response.Message = "Password Incorrect";       
            }
            else response.Data = CreateToken(user)   ;
            return response;


        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var serviceResponse = new ServiceResponse<int>();

            if (await UserExists(user.Username)){
                serviceResponse.Success = false;
                serviceResponse.Message = "User already exists";
                return serviceResponse;
            }
            
            CreatePasswordHash(password,out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            serviceResponse.Data = user.Id;
            return serviceResponse;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u=> u.Username == username) )
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        private bool VerifyPasswordHash(string password,  byte[] passwordHash,  byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return (computedHash.SequenceEqual(passwordHash));

            }

        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username)

            };

            var appSettingsToken = _config.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
            throw new Exception("Token is empty in AppSettings");
            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));
            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescription = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires= DateTime.Now.AddDays(1),
                SigningCredentials = cred

            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(tokenDescription);
            return handler.WriteToken(token);

            
        }
    }
}