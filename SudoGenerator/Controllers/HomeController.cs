using Microsoft.AspNet.Mvc;
using SudoGenerator.Models;

namespace SudoGenerator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(Configuration configuration)
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
    }
}
