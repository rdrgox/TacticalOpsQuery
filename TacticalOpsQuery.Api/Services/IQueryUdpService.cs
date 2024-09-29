using TacticalOpsQuery.Api.Models;

namespace TacticalOpsQuery.Api.Services;

public interface IQueryUdpService
{
    Task<ServerInfo?> QueryServerInfoAsync(string ip, int port, int timeOut);

    Task<List<Player>?> QueryPlayersAsync(string ip, int port, int timeOut);

    Task<ServerStatus?> QueryStatusAsync(string ip, int port, int timeOut);

    Task<List<TeamInfo>> QueryTeamsAsync(string ip, int port, int timeOut);
}
