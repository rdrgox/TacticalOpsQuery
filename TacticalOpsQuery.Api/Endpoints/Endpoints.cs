using TacticalOpsQuery.Api.Models;
using TacticalOpsQuery.Api.Services;

namespace TacticalOpsQuery.Api.Endpoints;

internal static class Endpoints
{
    internal static WebApplication AddEndpoints(this WebApplication app)
    {
        app.MapGet("/server-info", async (IQueryUdpService queryService, string ip, int queryPort, int timeOut) =>
        {
            var serverInfo = await queryService.QueryServerInfoAsync(ip, queryPort, timeOut);
            return serverInfo != null ? Results.Ok(serverInfo) : Results.StatusCode(500);
        })
        .WithName("GetServerInfo")
        .WithTags("Server")
        .Produces<ServerInfo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithOpenApi();

        app.MapGet("/players", async (IQueryUdpService queryService, string ip, int queryPort, int timeOut) =>
        {
            var players = await queryService.QueryPlayersAsync(ip, queryPort, timeOut);
            return players != null ? Results.Ok(players) : Results.StatusCode(500);
        })
        .WithName("GetPlayers")
        .WithTags("Players")
        .Produces<Player>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithOpenApi();

        app.MapGet("/status", async (IQueryUdpService queryService, string ip, int queryPort, int timeOut) =>
        {
            var status = await queryService.QueryStatusAsync(ip, queryPort, timeOut);
            return status != null ? Results.Ok(status) : Results.StatusCode(500);
        })
        .WithName("GetStatus")
        .WithTags("Status")
        .Produces<ServerStatus>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithOpenApi();

        app.MapGet("/team-info", async (IQueryUdpService queryService, string ip, int queryPort, int timeOut) =>
        {
            var result = await queryService.QueryTeamsAsync(ip, queryPort, timeOut);
            return result != null ? Results.Ok(result) : Results.StatusCode(500);
        })
        .WithName("GetTeams")
        .WithTags("Teams")
        .Produces<TeamInfo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithOpenApi();

        return app;
    }
}

