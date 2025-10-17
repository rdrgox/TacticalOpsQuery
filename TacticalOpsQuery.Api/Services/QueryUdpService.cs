using System.Net.Sockets;
using System.Text;
using TacticalOpsQuery.Api.Data;
using TacticalOpsQuery.Api.Models;

namespace TacticalOpsQuery.Api.Services;

public class QueryUdpService(DataContext context) : IQueryUdpService
{
    private const int MaxRetries = 3;

    public async Task<List<Player>?> QueryPlayersAsync(string serverIp, int queryPort, int timeOut)
    {
        var timeOutInMs = timeOut * 1000;
        var playersResponse = new StringBuilder();
        var receiving = true;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            using (var udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Connect(serverIp, queryPort + 1);
                    udpClient.Client.ReceiveTimeout = timeOutInMs;

                    byte[] playersCommand = Encoding.UTF8.GetBytes("\\players\\");
                    await udpClient.SendAsync(playersCommand, playersCommand.Length);

                    while (receiving)
                    {
                        var receiveTask = udpClient.ReceiveAsync();
                        var delayTask = Task.Delay(timeOutInMs);
                        var completedTask = await Task.WhenAny(receiveTask, delayTask);

                        if (completedTask == receiveTask)
                        {
                            var playersResult = await receiveTask;
                            //string responsePart = Encoding.UTF8.GetString(playersResult.Buffer);
                            string responsePart = Encoding.GetEncoding(1252).GetString(playersResult.Buffer);

                            playersResponse.Append(responsePart);

                            // Determina si es el último paquete de la respuesta
                            if (responsePart.Contains("final")) // Cambia esto según el marcador final en tu protocolo
                            {
                                receiving = false;
                            }
                        }
                        else
                        {
                            // Timeout, asume que no hay más paquetes
                            receiving = false;
                        }
                    }

                    // Procesa la respuesta completa después de recibir todos los paquetes
                    return ParsePlayers(playersResponse.ToString());
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"Timeout: {ex.Message}");
                    if (attempt == MaxRetries)
                    {
                        Console.WriteLine("Servidor no responde después de varios intentos.");
                        throw new Exception("Servidor no responde.");
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Error de socket: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }
        }

        return null;
    }

    public async Task<ServerInfo?> QueryServerInfoAsync(string serverIp, int queryPort, int timeOut)
    {
        var baseTimeoutMs = timeOut * 1000;
        var serverResponse = new StringBuilder();
        //var receiving = true;

        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            using var udpClient = new UdpClient();
            try
            {
                
                udpClient.Connect(serverIp, queryPort + 1);

                // Medimos latencia
                var infoCommand = Encoding.UTF8.GetBytes("\\status\\");
                var startTime = DateTime.UtcNow;
                await udpClient.SendAsync(infoCommand, infoCommand.Length);

                var receiving = true;
                long latencyMs = -1;
                
                
                while (receiving)
                {
                    // Timeout adaptativo: 3x latencia o timeout base
                    var timeoutMs = latencyMs > 0 ? (int)(latencyMs * 3) : baseTimeoutMs;
                    udpClient.Client.ReceiveTimeout = timeoutMs;

                    var receiveTask = udpClient.ReceiveAsync();
                    var delayTask = Task.Delay(timeoutMs);
                    var completedTask = await Task.WhenAny(receiveTask, delayTask);

                    if (completedTask == receiveTask)
                    {
                        var infoResult = await receiveTask;
                        if (latencyMs < 0)
                            latencyMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
                        
                        //string responsePart = Encoding.UTF8.GetString(infoResult.Buffer);
                        string responsePart = Encoding.GetEncoding(1252).GetString(infoResult.Buffer);
                        serverResponse.Append(responsePart);

                        if (responsePart.Contains("\\final\\"))
                            receiving = false;
                    }
                    else
                    {
                        // Timeout, asumimos fin de respuesta
                        receiving = false;
                    }
                }

                var info = ParseServerInfo(serverResponse.ToString());
                info.LatencyMs = latencyMs;
                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Intento {attempt} fallido: {ex.Message}");
                if (attempt == MaxRetries)
                    return null;
            }
            
        }

        return null;
    }

    public async Task<ServerStatus?> QueryStatusAsync(string serverIp, int queryPort, int timeOut)
    {
        var serverInfoTask = QueryServerInfoAsync(serverIp, queryPort, timeOut);
        var playersTask = QueryPlayersAsync(serverIp, queryPort, timeOut);

        await Task.WhenAll(serverInfoTask, playersTask);

        var serverInfo = await serverInfoTask;
        var players = await playersTask;

        if (serverInfo != null && players != null)
        {
            return new ServerStatus
            {
                ServerInfo = serverInfo,
                Players = players
            };
        }

        return null;
    }

    public async Task<List<TeamInfo>> QueryTeamsAsync(string serverIp, int queryPort, int timeOut)
    {
        var serverInfo = await QueryServerInfoAsync(serverIp, queryPort, timeOut);
        var players = await QueryPlayersAsync(serverIp, queryPort, timeOut);

        if (serverInfo == null || players == null)
        {
            return null!;
        }

        var teams = new Dictionary<int, TeamInfo>
        {
            { 0, new TeamInfo { Name = "Terrorists", Score = (int)serverInfo.ScoreTerrorists! } },
            { 1, new TeamInfo { Name = "Special Forces", Score = (int)serverInfo.ScoreSpecialForces! } }
        };

        foreach (var player in players)
        {
            if (player.Team.HasValue && teams.ContainsKey(player.Team.Value))
            {
                teams[player.Team.Value].Players.Add(player);
            }
        }

        return teams.Values.ToList();
    }

    public async Task SavePlayersAsync(List<Player> players)
    {
        foreach (var player in players)
        {
            if (string.IsNullOrWhiteSpace(player.Name)) continue;

            var existing = await context.PlayerStats.FindAsync(player.Name);

            if (existing == null)
            {
                context.PlayerStats.Add(new PlayerStat
                {
                    Name = player.Name,
                    Kills = player.Frags ?? 0,
                    Deaths = player.Deaths ?? 0,
                    Score = player.Score ?? 0,
                    KillsDeaths = (player.Frags ?? 0) - (player.Deaths ?? 0)
                });
            }
            else
            {
                existing.Kills = player.Frags ?? 0;
                existing.Deaths = player.Deaths ?? 0;
                existing.Score = player.Score ?? 0;
                existing.KillsDeaths = (player.Frags ?? 0) - (player.Deaths ?? 0);
            }
        }

        await context.SaveChangesAsync();
    }

    private static ServerInfo ParseServerInfo(string response)
    {
        var serverInfo = new ServerInfo();
        var parts = response.Split('\\', StringSplitOptions.RemoveEmptyEntries);

        serverInfo.Hostname = GetValue(parts, "hostname");
        serverInfo.Hostport = SafeParseInt(GetValue(parts, "hostport"));
        serverInfo.GameVer = SafeParseInt(GetValue(parts, "gamever"));
        serverInfo.MapTitle = GetValue(parts, "maptitle");
        serverInfo.MapName = GetValue(parts, "mapname");
        serverInfo.MaxPlayers = SafeParseInt(GetValue(parts, "maxplayers"));
        serverInfo.NumPlayers = SafeParseInt(GetValue(parts, "numplayers"));
        serverInfo.GameType = GetValue(parts, "gametype");
        serverInfo.ScoreTerrorists = SafeParseInt(GetValue(parts, "tscore_0"));
        serverInfo.ScoreSpecialForces = SafeParseInt(GetValue(parts, "tscore_1"));
        serverInfo.RoundNumber = SafeParseInt(GetValue(parts, "roundnumber"));
        serverInfo.LastWinningTeam = SafeParseInt(GetValue(parts, "lastwinningteam"));
        serverInfo.Mutators = GetValue(parts, "mutators");
        serverInfo.TimeLimit = SafeParseInt(GetValue(parts, "timelimit"));
        serverInfo.FriendlyFire = GetValue(parts, "friendlyfire");
        serverInfo.TostVersion = GetValue(parts, "tostversion");
        serverInfo.AdminName = GetValue(parts, "AdminName");
        serverInfo.AdminEmail = GetValue(parts, "AdminEMail");
        serverInfo.Password = GetValue(parts, "password");
        return serverInfo;
    }

    private static List<Player> ParsePlayers(string response)
    {
        var players = new List<Player>();
        var parts = response.Split('\\', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].StartsWith("player_"))
            {
                var playerIndex = parts[i].Split('_')[1];

                if (string.IsNullOrEmpty(playerIndex)) continue;
                var player = new Player
                {
                    Name = GetValue(parts, $"player_{playerIndex}"),
                    Frags = SafeParseInt(GetValue(parts, $"frags_{playerIndex}")),
                    Deaths = SafeParseInt(GetValue(parts, $"deaths_{playerIndex}")),
                    Score = SafeParseInt(GetValue(parts, $"score_{playerIndex}")),
                    Ping = SafeParseInt(GetValue(parts, $"ping_{playerIndex}")),
                    Team = SafeParseInt(GetValue(parts, $"team_{playerIndex}")),
                    Mesh = GetValue(parts, $"mesh_{playerIndex}"),
                    Skin = GetValue(parts, $"skin_{playerIndex}"),
                    Health = SafeParseInt(GetValue(parts, $"health_{playerIndex}")),
                };
                players.Add(player);
            }
        }

        return players;
    }

    private static string GetValue(string[] parts, string key)
    {
        for (var i = 0; i < parts.Length - 1; i++)
        {
            if (parts[i] == key)
            {
                return parts[i + 1];
            }
        }

        return null!;
    }

    private static int SafeParseInt(string value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }
    
}