using Microsoft.AspNet.Mvc;
using SudoGenerator.Models;

namespace SudoGenerator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new Configuration());
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
    }
}
