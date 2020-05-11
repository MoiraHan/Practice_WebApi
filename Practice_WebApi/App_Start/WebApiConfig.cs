using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Practice_WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務
            config.EnableCors();

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 如果 Json 序列化是使用 Json.Net 序列化 ( 應該是微軟原生的Json(?) )，
            // 這行程式可以忽略循環引用 ( 就是 Model 中有相同類型的 Property，導致進行 Json 序列化時無限迴圈 )
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
