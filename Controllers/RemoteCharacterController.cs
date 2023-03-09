using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemoteCharacterController : ControllerBase
    {
                private readonly HttpClient _client;

                        public RemoteCharacterController(HttpClient client){
            _client = client;
        }


        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Character>>>> Get() {

            var response = await _client.GetAsync("https://sktestinsights-fa.azurewebsites.net/api/HttpTrigger1");
            var str = await response.Content.ReadAsStringAsync();
            return  Ok(str);

        }

    }
}