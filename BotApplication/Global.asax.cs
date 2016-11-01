using System;
using System.IO;
using System.Web;
using System.Web.Http;

namespace BotApplication
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
