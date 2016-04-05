using Microsoft.AspNet.Mvc;
using SudoGenerator.App.Classes;

namespace SudoGenerator.App.Controllers
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
