using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.Controllers
{
    public class Fallback : Controller
    {
        //this is basically what we need to do for ASP core MVC to redirect our request and send
        //it back to index.html which is our angular application and we're effectively passing
        //off the angular router to deal with.
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }
    }
}
