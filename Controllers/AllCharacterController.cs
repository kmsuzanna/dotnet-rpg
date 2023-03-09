using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NewRelic.Api.Agent;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllCharacterController : ControllerBase
    {

        private readonly HttpClient _client;
        private readonly ILogger _loggeer;

        public AllCharacterController(HttpClient client){
            _client = client;
        }


        [HttpGet]
        [Transaction(Web = true)]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get() {
            
            StringValues headerValue;
            Request.Headers.TryGetValue("newrelic",   out  headerValue); 
            
            var response = await _client.GetAsync("https://rpg-fa2.azurewebsites.net/api/GetAllCharacters?name=suz");
            var str = response.Content.ReadFromJsonAsync<List<Character>>();
            var car = new Character();
            car.Name= headerValue;
            return  Ok(str);

        }
    }
}