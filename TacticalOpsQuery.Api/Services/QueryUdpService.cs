﻿using System.Net.Sockets;
using System.Text;
using TacticalOpsQuery.Api.Models;

namespace TacticalOpsQuery.Api.Services;

public class QueryUdpService : IQueryUdpService
{
    private const int MaxRetries = 3;

    public async Task<List<Player>?> QueryPlayersAsync(string serverIP, int queryPort, int timeOut)
    {
        int timeOutInMS = timeOut * 1000;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            using (var udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Connect(serverIP, queryPort);
                    udpClient.Client.ReceiveTimeout = timeOutInMS;

                    // Lógica para consultar lista de jugadores
                    byte[] playersCommand = Encoding.UTF8.GetBytes("\\players\\");
                    await udpClient.SendAsync(playersCommand, playersCommand.Length);

                    var receiveTask = udpClient.ReceiveAsync();

                    if (await Task.WhenAny(receiveTask, Task.Delay(timeOutInMS)) == receiveTask)
                    {
                        //var playersResult = await udpClient.ReceiveAsync();
                        var playersResult = await receiveTask;
                        string playersResponse = Encoding.UTF8.GetString(playersResult.Buffer);
                        return ParsePlayers(playersResponse);
                    }
                    else
                    {
                        throw new TimeoutException($"No response from server on attempt {attempt}.");
                    }
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

    public async Task<ServerInfo?> QueryServerInfoAsync(string serverIP, int queryPort, int timeOut)
    {
        int timeOutInMS = timeOut * 1000;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            using (var udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Connect(serverIP, queryPort);
                    udpClient.Client.ReceiveTimeout = timeOutInMS;

                    // Lógica para consultar información del servidor
                    byte[] infoCommand = Encoding.UTF8.GetBytes("\\status\\");
                    await udpClient.SendAsync(infoCommand, infoCommand.Length);

                    var receiveTask = udpClient.ReceiveAsync();

                    if (await Task.WhenAny(receiveTask, Task.Delay(timeOutInMS)) == receiveTask)
                    {
                        var infoResult = await receiveTask;
                        string infoResponse = Encoding.UTF8.GetString(infoResult.Buffer);
                        return ParseServerInfo(infoResponse);
                    }
                    else
                    {
                        throw new TimeoutException($"No response from server on attempt {attempt}.");
                    }
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
                    // Manejar excepción específica de socket
                    Console.WriteLine($"Error de socket: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    // Manejar otras excepciones de manera genérica
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }
        }
        return null;
    }

    public async Task<ServerStatus?> QueryStatusAsync(string serverIP, int queryPort, int timeOut)
    {
        var serverInfoTask = QueryServerInfoAsync(serverIP, queryPort, timeOut);
        var playersTask = QueryPlayersAsync(serverIP, queryPort, timeOut);

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

    public async Task<List<TeamInfo>?> QueryTeamsAsync(string serverIP, int queryPort, int timeOut)
    {
        var serverInfo = await QueryServerInfoAsync(serverIP, queryPort, timeOut);
        var players = await QueryPlayersAsync(serverIP, queryPort, timeOut);

        if (serverInfo == null || players == null)
        {
            return null;
        }

        var teams = new Dictionary<int, TeamInfo>
            {
                { 0, new TeamInfo { Name = "Terrorists", Score = (int)serverInfo.ScoreTerrorists } },
                { 1, new TeamInfo { Name = "Special Forces", Score = (int) serverInfo.ScoreSpecialForces } }
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
        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (parts[i] == key)
            {
                return parts[i + 1];
            }
        }
        return null;
    }

    private static int SafeParseInt(string value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }
}


