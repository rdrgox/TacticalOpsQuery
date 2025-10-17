using TacticalOpsQuery.Api.Models;
using TacticalOpsQuery.Api.Services;

namespace TacticalOpsQuery.Api.Endpoints;

internal static class Endpoints
{
    internal static WebApplication AddEndpoints(this WebApplication app)
    {
        app.MapGet("/server-info", async (IQueryUdpService queryService, string ip, int port, int timeOut) =>
        {
            var serverInfo = await queryService.QueryServerInfoAsync(ip, port, timeOut);
            return serverInfo != null ? Results.Ok(serverInfo) : Results.StatusCode(500);
        })
        .WithName("GetServerInfo")
        .WithTags("Server")
        .Produces<ServerInfo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Fixed")
        .WithOpenApi();

        app.MapGet("/players", async (IQueryUdpService queryService, string ip, int port, int timeOut) =>
        {
            var players = await queryService.QueryPlayersAsync(ip, port, timeOut);
            return players != null ? Results.Ok(players) : Results.StatusCode(500);
        })
        .WithName("GetPlayers")
        .WithTags("Players")
        .Produces<Player>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Fixed")
        .WithOpenApi();

        app.MapGet("/status", async (IQueryUdpService queryService, string ip, int port, int timeOut) =>
        {
            var status = await queryService.QueryStatusAsync(ip, port, timeOut);
            return status != null ? Results.Ok(status) : Results.StatusCode(500);
        })
        .WithName("GetStatus")
        .WithTags("Status")
        .Produces<ServerStatus>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Fixed")
        .WithOpenApi();

        app.MapGet("/team-info", async (IQueryUdpService queryService, string ip, int port, int timeOut) =>
        {
            var result = await queryService.QueryTeamsAsync(ip, port, timeOut);
            return result != null ? Results.Ok(result) : Results.StatusCode(500);
        })
        .WithName("GetTeams")
        .WithTags("Teams")
        .Produces<TeamInfo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Fixed")
        .WithOpenApi();

        /*app.MapPost("/save-players", async (IQueryUdpService queryService, string ip, int port, int timeOut) =>
        {
            var players = await queryService.QueryPlayersAsync(ip, port, timeOut);

            if (players == null)
                return Results.StatusCode(500);

            await queryService.SavePlayersAsync(players);

            return Results.Ok("Datos guardados correctamente.");
        })
        .WithName("Postplayers")
        .WithTags("PlayerStats")
        .Produces<Player>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithOpenApi();*/

        return app;
    }
}