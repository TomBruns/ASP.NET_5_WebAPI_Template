using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PayAway.WebAPI.Entities.v1;

namespace PayAway.WebAPI.Entities.v2
{
    /// <summary>
    /// Weather Forecast response
    /// </summary>
    public class WeatherForecast2MBE
    {
        const decimal CELSIUS_TO_KELVIN = 274.15M;

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date UTC.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the temperature in Celsius.
        /// </summary>
        /// <value>The temperature c.</value>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Gets the temperature in Fahrenheit
        /// </summary>
        /// <value>The temperature f.</value>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Gets the temperature in Kelvin
        /// </summary>
        /// <value>The temperature k.</value>
        public int TemperatureK => (int)(TemperatureC * CELSIUS_TO_KELVIN);

        /// <summary>
        /// Gets or sets the Weather Forecast.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }
    }
}
