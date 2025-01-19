using Microsoft.AspNetCore.Mvc;
using Api.Gateway;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using Api.Gateway.Models;

namespace Api.Gateway.Controllers
{
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly Urls _urls;

        public SubscriptionsController(HttpClient httpClient, IOptions<Urls> urlsOptions)
        {
            _httpClient = httpClient;
            _urls = urlsOptions.Value;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _httpClient.GetAsync($"{_urls.MemberSubscriptions}/api/subscriptions");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var subscriptions = await response.Content.ReadFromJsonAsync<object>();
            return Ok(subscriptions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _httpClient.GetAsync($"{_urls.MemberSubscriptions}/api/subscriptions/{id}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var subscription = await response.Content.ReadFromJsonAsync<object>();
            return Ok(subscription);
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] MemberSubscription subscription)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_urls.MemberSubscriptions}/api/subscriptions", subscription);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var createdSubscription = await response.Content.ReadFromJsonAsync<object>();
            return Created(string.Empty, createdSubscription);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] object subscription)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_urls.MemberSubscriptions}/api/subscriptions/{id}", subscription);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_urls.MemberSubscriptions}/api/subscriptions/{id}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return NoContent();
        }
    }
}

