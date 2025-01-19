using Api.Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api.Gateway.Controllers
{
    [ApiController]
    [Route("api/members")]
    public class MembersGatewayController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly Urls _urls;

        public MembersGatewayController(HttpClient httpClient, IOptions<Urls> urlsOptions)
        {
            _httpClient = httpClient;
            _urls = urlsOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var response = await _httpClient.GetAsync($"{_urls.Members}/api/members");
            return await HandleResponse(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            var response = await _httpClient.GetAsync($"{_urls.Members}/api/members/{id}");
            return await HandleResponse(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Member request)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_urls.Members}/api/members/register", jsonContent);
            return await HandleResponse(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] object updatedMember)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedMember), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_urls.Members}/api/members/{id}", jsonContent);
            return await HandleResponse(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_urls.Members}/api/members/{id}");
            return await HandleResponse(response);
        }

        private async Task<IActionResult> HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return Content(responseData, response.Content.Headers.ContentType.ToString());
            }
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
