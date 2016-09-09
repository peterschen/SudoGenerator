using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;

namespace SudoGenerator.Controllers
{
    public class ExceptionController : Controller
    {
        public static TelemetryClient telemetry = new TelemetryClient();
        
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
