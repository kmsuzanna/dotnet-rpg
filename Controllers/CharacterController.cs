using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ILogger<CharacterController> _logger;
        private readonly ICharacterService _characterService;



        public CharacterController(ILogger<CharacterController> logger, ICharacterService characterService)
        {
            _logger = logger;
            _characterService = characterService;
        }


        [AllowAnonymous]
        [Trace]
        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get() {
            return  Ok(await _characterService.GetListOfCharacters());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id) {
            return  Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> SaveCharacter(AddCharacterDto character) {
            
            return  Ok( await _characterService.AddCharacter(character));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto character) {
            var response = await _characterService.UpdateCharacter(character);
            if (response.Data is null){
                return NotFound(response);
            }
            return  Ok(response );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> DeleteCharacter(int id) {
            var response = await _characterService.DeleteCharacter(id);
            if (response.Data is null){
                return NotFound(response);
            }
            return  Ok(response );
        }
    }
}