using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Routing;

namespace HorseShowAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
          name: "RatingApi",
          routeTemplate: "api/{controller}/{action}/{ratingJason}",
          defaults: new { action = "get", id = RouteParameter.Optional }
          );
            config.Routes.MapHttpRoute(
            name: "ClassesApi",
            routeTemplate: "api/{controller}/{action}/{flag}",
            defaults: new { action = "get", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
            name: "HorsesApi",
            routeTemplate: "api/{controller}/{action}/{showclassid}",
            defaults: new { action = "get", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
         name: "ClassCompletedApi",
         routeTemplate: "api/{controller}/{action}/{classCompleteFlag}/{classid}",
         defaults: new { action = "get", id = RouteParameter.Optional }
         );
            config.Routes.MapHttpRoute(
         name: "JudgeApi",
         routeTemplate: "api/{controller}/{action}/{IpadUdid}",
         defaults: new { action = "get", id = RouteParameter.Optional }
         );
            config.Routes.MapHttpRoute(
        name: "SetConfigApi",
        routeTemplate: "api/{controller}/{action}/{key}/{value}/{udid}",
        defaults: new { action = "get", id = RouteParameter.Optional }
        );
            config.Routes.MapHttpRoute(
            name: "WinnerType",
            routeTemplate: "api/{controller}/{action}/{flag}/{championshipid}/{methodid}/{showid}",
            defaults: new { action = "get", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
            name: "ChampHorseList",
            routeTemplate: "api/{controller}/{action}/{championshipid}/{showid}/{winnertypeid}/{methodid}",
            defaults: new { action = "get", id = RouteParameter.Optional }
             );
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

        }
    }
}
