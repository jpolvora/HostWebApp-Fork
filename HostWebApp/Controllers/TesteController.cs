using System.Web;
using System.Web.Mvc;
using MvcLib.FsDump;

namespace HostWebApp.Controllers
{
    //teste controller for building from Db.
    //dump into database and see how it works.
    //build action = None

    public class TesteController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        public ActionResult About()
        {
            ViewData["Message"] = "Your application description page..(from TesteController)";

            return View("~/Views/Home/About.cshtml");
        }

        public ActionResult Contact()
        {
            ViewData["Message"] = "Your contact page. (from TesteController)";

            return View("~/Views/Home/Contact.cshtml");
        }

        public ActionResult Reset()
        {            
            HttpRuntime.UnloadAppDomain();
            return RedirectToAction("Index", new { q = "Reset success" });
        }

        public ActionResult Refresh()
        {
            DbToLocal.Execute();
            
            return RedirectToAction("Index", new { q = "Refresh success" });
        }
    }
}





