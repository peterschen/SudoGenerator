using System;
using Microsoft.AspNetCore.Mvc;
using SudoGenerator.Classes;
using SudoGenerator.Models;
using Microsoft.ApplicationInsights;

namespace SudoGenerator.Controllers
{
    public class ExceptionController : Controller
    {
        public static TelemetryClient telemetry = new TelemetryClient();

        [Route("Exception/Index")]
        public IActionResult Index()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                // Track exception and rethrow error
                telemetry.TrackException(e);
                throw e;
            }
        }
    }
}
