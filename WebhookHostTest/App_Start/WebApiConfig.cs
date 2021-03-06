﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebhookHostTest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Attribute routing.
            config.MapHttpAttributeRoutes();

            // Web API routes
            //config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.InitializeReceiveGenericJsonWebHooks();

            //This is the registration of the EasyPost Webhook. 
            config.InitializeReceiveEasyPostWebHooks();
        }
    }
}
