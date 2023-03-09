using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Models;
using NewRelic.Api.Agent;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {

        private readonly IMapper _mapper;
        private readonly DataContext _context;

        private readonly IHttpContextAccessor _httpContext;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _context = context;
            _httpContext = httpContext;
            
        }
        private int GetUserId() => 
             int.Parse(_httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var _character = _mapper.Map<Character>(character); 
            _character.User = await _context.Users.FirstOrDefaultAsync(u=>u.Id == GetUserId());

             _context.Characters.Add(_character);
            await _context.SaveChangesAsync();

            var characters = await _context.Characters.ToListAsync();

            serviceResponse.Data = await _context.Characters.Where(c=>c.User!.Id==GetUserId()).Select(c=>_mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            var dbCharacter  = await _context.Characters.FirstOrDefaultAsync(c=>c.Id==id && c.User!.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);

            return serviceResponse;
            

            
        }

        [Trace]
        public async Task<ServiceResponse<List<GetCharacterDto>>> GetListOfCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            // var userId = GetUserId();

            var dbCharacters = await _context.Characters.Where(c=>c.User!.Id==3).ToListAsync();

            serviceResponse.Data = dbCharacters.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToList();

             return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedChar){
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try{

            var character = await _context.Characters.FirstOrDefaultAsync(c=> c.Id == updatedChar.Id);
            character.Name = updatedChar.Name;
            character.Intelligence = updatedChar.Intelligence;
            character.HitPoints = updatedChar.HitPoints;
            character.Strength = updatedChar.Strength;
            character.Class = updatedChar.Class;
            character.Defense = updatedChar.Defense;
            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);

 
            }
            catch(Exception ex){
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }


            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {

            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var character = _context.Characters.First(c=>c.Id==id);
                if (character is null){
                    throw new Exception("Character witht '${id}' not foundd");
                }

                _context.Remove(character);
                await _context.SaveChangesAsync();
                 serviceResponse.Data = await _context.Characters.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToListAsync();



            }
            catch (System.Exception ex)
            {
                
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;            }

                return serviceResponse;

        }
    }
}