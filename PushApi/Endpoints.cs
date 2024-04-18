using Microsoft.AspNetCore.Mvc;
using Shiny.Extensions.Push;

namespace PushApi;


public static class Endpoints
{
    public static void RegisterEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/push/send",
            async (
                [FromBody] PushSendRequest request,
                [FromServices] IPushManager push
            ) =>
            {
                await push.Send(
                    new Notification
                    {
                        Title = request.NotificationTitle,
                        Message = request.NotificationMessage
                    },
                    request.Filter
                );
            }
        );

        app.MapPost(
            "/push/list",
            async (
                [FromBody] Filter filter,
                [FromServices] IPushManager push
            ) =>
            {
                var results = await push.GetRegistrations(filter);
                return Results.Ok(results);
            }
        );
    }
}


public record PushSendRequest(
    string NotificationTitle,
    string NotificationMessage,
    Filter? Filter = null
);