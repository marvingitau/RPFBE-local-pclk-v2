using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFBE
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            // BeginInvoke(context);
            var Audience = _configuration["Jwt:ValidAudience"];
            //capture the origin header to compare where the call is coming from
            var headers = context.Request.Headers;
            var currAudience = headers["Origin"];

    
            if (Audience != currAudience)
            {
                context.Response.StatusCode = 406;
                await context.Response.WriteAsync("Not Acceptable Request");

            }else
            {

            await _next.Invoke(context);
            }
          

        }

        private async void BeginInvoke(HttpContext context)
        {
            var Audience = _configuration["Jwt:ValidAudience"];
            //capture the origin header to compare where the call is coming from
            var headers = context.Request.Headers;
            var currAudience = headers["Origin"];

      

            if (Audience != currAudience)
            {
                context.Response.StatusCode = 406;
                await context.Response.WriteAsync("Not Acceptable Request");

                //await _next.Invoke(context);
            }



            if (context.Request.Method != "OPTIONS" && context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Not Authenticated");
            }
            await _next.Invoke(context);
        }
    }
}
