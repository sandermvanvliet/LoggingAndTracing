using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace Demo.CarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public CarsController(ILogger logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("user-api");
        }

        [HttpPost("{vin}/data")]
        public async Task<IActionResult> Post(string vin, [FromBody] CarData value)
        {
            _logger.Information($"Received car data for VIN: {vin}");

            try
            {
                var response = await _httpClient.GetAsync($"/api/users/by-vin/{vin}");

                if (response.IsSuccessStatusCode)
                {
                    var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

                    _logger.Information($"User for VIN {vin} is {user.Name}");

                    return Ok();
                }

                _logger.Error($"Could not retrieve user for VIN {vin} because response status code was {response.StatusCode}");

                return StatusCode((int)HttpStatusCode.BadGateway);
            }
            catch (OperationCanceledException)
            {
                _logger.Error($"Could not retrieve user for VIN {vin} because the operation timed out");

                return StatusCode((int)HttpStatusCode.GatewayTimeout);
            }
            catch (Exception ex)
            {
                _logger.Error($"Could not retrieve user for VIN {vin} because {ex.Message}");
                
                return StatusCode((int)HttpStatusCode.BadGateway);
            }
        }
    }
}