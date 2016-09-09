using Microsoft.AspNetCore.Mvc;
using SudoGenerator.Classes;
using SudoGenerator.Models;

namespace SudoGenerator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new Configuration { ProductVersion = "2012-r2" });
        }

        public IActionResult Configuration(Configuration configuration)
        {
            configuration.Generate();
            return View(configuration);
        }

        public IActionResult Imprint()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Data(string type, string productVersion)
        {
            JsonResult data = null;

            if (type == "os")
            {
                if (productVersion == "2012-rtm")
                {
                    data = Json(OperatingSystem.SC2012RTM);
                }
                else if (productVersion == "2012-r2")
                {
                    data = Json(OperatingSystem.SC2012R2);
                }
            }

            return data;
        }
    }
}
