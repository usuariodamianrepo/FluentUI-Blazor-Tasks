using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class WeatherForecastController : ControllerBase
    {
        private readonly int milliseconds = 100;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecastDTO>> Get()
        {
            //Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            await Task.Delay(milliseconds);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastDTO
            {
                Id = index,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("{id}")]
        public async Task<WeatherForecastDTO> Get(int id)
        {
            await Task.Delay(milliseconds);
            return new WeatherForecastDTO
            {
                Id = id,
                Date = DateTime.Now.AddDays(id),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Post([FromBody] WeatherForecastDTO weather)
        {
            await Task.Delay(milliseconds);
            try
            {
                return Ok(new GeneralResponse(true, $"Weather with Id: {weather.Id} created successfully.")); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> Put(int id, [FromBody] WeatherForecastDTO weather)
        {
            await Task.Delay(milliseconds);
            try
            {
                return Ok(new GeneralResponse(true, $"Weather with Id: {weather.Id} update successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(int id)
        {
            await Task.Delay(milliseconds);
            try
            {
                return Ok(new GeneralResponse(true, $"Weather with Id: {id} delete successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
