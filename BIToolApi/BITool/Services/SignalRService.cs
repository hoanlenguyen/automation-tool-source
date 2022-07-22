using BITool.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BITool.Services
{
    public static class SignalRService
    {
        public static void AddSignalRService(this WebApplication app)
        {
            app.MapGet("signalR/test", [Authorize]
            async Task<IResult> (
            [FromServices] IHubContext<HubClient> hubContext,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromQuery] string connectionId) =>
            {
                var userIdSr = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"userIdSr {userIdSr}");
                Console.WriteLine($"connectionId {connectionId}");
                if (hubContext.Clients != null)
                {
                    var client = hubContext.Clients.Client(connectionId);
                    if (client != null)
                    {
                        await client.SendAsync("getPlayer", new { isOk = true });
                    }
                }
                else
                    Console.WriteLine("No client");
                return Results.Ok();
            });
        }
    }
}