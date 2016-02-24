namespace SpeakImageOverlay.Web.Controllers
{
    using System.Web.Mvc;
    using Glass.Mapper.Sc.Web.Mvc;
    using Web.Models;

    public class HomeController : GlassController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImageOverlay()
        {
            return View(ContextItem == null ? null : SitecoreContext.Cast<IPage>(ContextItem));
        }
    }
}