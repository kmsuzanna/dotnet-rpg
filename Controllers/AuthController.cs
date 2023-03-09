using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request){

            var response = await _authRepo.Register(new User{Username = request.Username}, request.Password);

            if (response.Success)
                return Ok(response);
            return BadRequest(response);


        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request){

        var response = await _authRepo.Login(request.Username, request.Password);

        if (response.Success) return Ok(response);
        else return BadRequest(response);




        }
    }
}