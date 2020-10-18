using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PayAway.WebAPI.Entities.v2;

namespace PayAway.WebAPI.Controllers.v2
{
    /// <summary>
    /// This is v2 of the WeatherForecast Controller
    /// </summary>
    /// <remarks>
    /// This version implements a numer of enhancements over v1
    /// - Swagger
    ///     adds version specific documentation at the top of the page (incl a chg notes table)
    ///     correctly indicates the media type of the response as JSON
    ///     adds comments at the method level
    ///     adds comments at the method param level
    ///     correctly indicates the possible HTTP response codes (ex 200 and 400)
    ///     returns a RFC7807 compliant ProblemDetails return object on errors
    ///     return entities have field level descriptions
    /// - General
    ///     Methods chg'd to return ActionResult so we can control response codes
    ///     Methods return ProblemDetails for all exceptions
    ///     Special care to return the logically correct error code in place of a HTTP 500 
    /// </remarks>
    [ApiController]
    [Route("api/v2/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the weather forecast for the next 5 days
        /// </summary>
        /// <returns>IEnumerable&lt;WeatherForecast2MBE&gt;.</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast2MBE>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<WeatherForecast2MBE>> Get()
        {
            return Get(5);
        }

        /// <summary>
        /// Gets the weather forecast for the specified number of days
        /// </summary>
        /// <param name="numberOfDays">The number of days.</param>
        /// <returns>IEnumerable&lt;WeatherForecast2MBE&gt;.</returns>
        [HttpGet("days/{numberOfDays:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast2MBE>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<WeatherForecast2MBE>> Get(int numberOfDays)
        {
            if (numberOfDays < 1 || numberOfDays > 10)
            {
                // the exception will automatically be wrapped in a RFC7807 compliant ProblemDetails return object (so it can be parsed by the caller)

                // this will return a HTTP 500 (Internal Server Error) which is not logically correct
                //throw new ArgumentOutOfRangeException(nameof(numberOfDays), @"You must supply a value between 1 and 10 days inclusive.");

                // this will return a HTTP 400 (Bad Request) which is logically correct
                return BadRequest(new ArgumentOutOfRangeException(nameof(numberOfDays), @"You must supply a value between 1 and 10 days inclusive."));
            }

            var rng = new Random();

            return Enumerable.Range(1, numberOfDays).Select(index => new WeatherForecast2MBE
            {
                Date = DateTime.Now.Date.ToUniversalTime().AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
