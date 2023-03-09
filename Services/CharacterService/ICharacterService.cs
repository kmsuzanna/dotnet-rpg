using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        public Task<ServiceResponse<List<GetCharacterDto>>> GetListOfCharacters();
        public Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);

        public Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto character);

        public Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedChar);

        public Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);


    }
}