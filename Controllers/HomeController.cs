using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProfanityScanner.Models;
using ProfanityScanner.Models.Classes;

namespace ProfanityScanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Trie profaneTrie;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            profaneTrie = new Trie(env);
            profaneTrie.InsertFile("Sources", "ProfaneWords.txt");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new Scanner());
        }

        [HttpPost]
        public IActionResult Index(string inputText)
        {
            Scanner scanner = new Scanner();
            if (inputText == null)
                return View(scanner);
            string firstPass = scanner.Censor(inputText, profaneTrie.FindProfanity(inputText));
            string secondPass = Scanner.Substitute(firstPass);
            scanner.output = scanner.Censor(firstPass, profaneTrie.FindProfanity(secondPass));

            return View(scanner);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
