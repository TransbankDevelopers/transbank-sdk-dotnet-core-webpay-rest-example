using Microsoft.AspNetCore.Mvc;

namespace transbanksdkdotnetrestexample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Transbank .Net SDK";
            return View();
        }
    }
}