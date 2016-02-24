namespace SpeakImageOverlay.Web
{
    using System.Web.Mvc;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
