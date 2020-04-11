using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_3.Services;

namespace Task_3.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;


        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request != null)
            {

                string path = context.Request.Path;
                string method = context.Request.Method; 
                string queryString = context.Request.QueryString.ToString();
                string bodyStr = "";

                context.Request.EnableBuffering();

                using (StreamReader reader=new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }


                report(path + "\n" + method + "\n" + queryString + "\n" + bodyStr + "\n");
            }


            if(_next!=null) await _next(context);
        }

        public static void report(string log)
        {
            using var sw = new StreamWriter(@"requestsLog.txt", true);
            sw.WriteLine(log);
        }

    }
}
