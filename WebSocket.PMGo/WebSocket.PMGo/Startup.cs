using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebSocket = System.Net.WebSockets.WebSocket;

namespace WebSocket.PMGo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseWebSockets();

            app.Use(async (context, next) =>
                    {
                        if (context.Request.Path == "/ws")
                        {
                            if (context.WebSockets.IsWebSocketRequest)
                            {
                                System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                                webSockets.Add(webSocket);
                                await Echo(context, webSocket);
                            }
                            else
                            {
                                context.Response.StatusCode = 400;
                            }
                        }
                        else
                        {
                            await next();
                        }

                    });

            app.UseFileServer();
        }

        List<System.Net.WebSockets.WebSocket> webSockets = new List<System.Net.WebSockets.WebSocket>();

        private async Task Echo(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            WebSocketReceiveResult result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {

                foreach (System.Net.WebSockets.WebSocket socket in this.webSockets)
                {
                    await socket.SendAsync(
                                           new ArraySegment<byte>(buffer, 0, result.Count),
                                           result.MessageType,
                                           result.EndOfMessage,
                                           CancellationToken.None);
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                                       result.CloseStatus.Value,
                                       result.CloseStatusDescription,
                                       CancellationToken.None);
        }
    }
}
