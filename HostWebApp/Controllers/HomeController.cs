using System.Web;
using System.Web.Mvc;
using MvcLib.Common;
using MvcLib.FsDump;

namespace HostWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewData["Message"] = "Your application description page.....";

            return View();
        }

        public ActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public ActionResult Reset()
        {
            return View();
        }

        [HttpPost, ActionName("Reset")]
        public ActionResult Post()
        {
            HttpRuntime.UnloadAppDomain();
            return new HttpStatusCodeResult(200);
        }

        public ActionResult Refresh()
        {
            DbToLocal.Execute();

            return RedirectToAction("Index", new { q = "Refresh success" });
        }

        public ActionResult CatchAll(string url)
        {
            return new HttpStatusCodeResult(404, "Caminho sem rota: {0}".Fmt(url));
        }
    }
}





