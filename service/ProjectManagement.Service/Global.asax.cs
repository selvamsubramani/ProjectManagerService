﻿using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace ProjectManagement.Service
{
    [ExcludeFromCodeCoverage]
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
