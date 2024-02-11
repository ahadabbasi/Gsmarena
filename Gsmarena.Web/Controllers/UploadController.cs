using Microsoft.AspNetCore.Mvc;

namespace Gsmarena.Web.Controllers;

public class UploadController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}