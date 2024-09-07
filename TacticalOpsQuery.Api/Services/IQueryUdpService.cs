using TacticalOpsQuery.Api.Models;

namespace TacticalOpsQuery.Api.Services;

public interface IQueryUdpService
{
    Task<ServerInfo?> QueryServerInfoAsync(string ip, int port);

    Task<List<Player>?> QueryPlayersAsync(string ip, int port);

    Task<ServerStatus?> QueryStatusAsync(string ip, int port);

    Task<List<TeamInfo>> QueryTeamsAsync(string ip, int port);
}
