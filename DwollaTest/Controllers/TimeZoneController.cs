using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Collections;


namespace DwollaTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimeZoneController : ControllerBase
    {
        private readonly ILogger<TimeZoneController> _logger;

        public TimeZoneController(ILogger<TimeZoneController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get the current UTC time, and optionally, the current time at the given UTC offset
        /// </summary>
        /// <param name="offset">A valid UTC offset in the format +##:## of -##:##</param>
        /// <returns>A JSON response object with the currentTime and (if requested) the offsetTime</returns>
        [HttpGet(Name = "time")]
        public ActionResult<TimeResponse> Get(string offset = "")
        {
            TimeResponse time = new TimeResponse();
            time.currentTime = DateTime.UtcNow;
            if(!string.IsNullOrEmpty(offset))
            {
                //check for valid offset
                DateTime adjusted = ApplyOffset(offset, time.currentTime);
                if(adjusted.Year == 1)
                {
                    return BadRequest("Invalid UTC Offset.");
                }
                else
                {
                    time.adjustedTime = adjusted;
                }
            }

            return Ok(time);
        }

        private DateTime ApplyOffset(string offset, DateTime time)
        {

            DateTimeOffset dto;
            if(DateTimeOffset.TryParse(time.ToShortDateString() + " " + time.ToShortTimeString() + " " + offset, null, out dto))
            {
                TimeSpan parsedOffset = dto.Offset;
                return dto.DateTime.Add(parsedOffset);
            }
            else
            {
                return new DateTime(1,1,1);
            }

        }
    }
}
