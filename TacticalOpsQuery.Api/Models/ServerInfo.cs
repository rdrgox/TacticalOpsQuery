using Swashbuckle.AspNetCore.Annotations;

namespace TacticalOpsQuery.Api.Models;

[SwaggerSchema(Description = "Detailed server information")]
public class ServerInfo
{
    [SwaggerSchema("name of the specific server")]
    public string Hostname { get; set; } = string.Empty;

    [SwaggerSchema("hostport to join the server")]
    public int? Hostport { get; set; }

    [SwaggerSchema("game version of the server")]
    public int? GameVer { get; set; }

    [SwaggerSchema("title or description of current map")]
    public string MapTitle { get; set; } = string.Empty;

    [SwaggerSchema("filename of current map")]
    public string MapName { get; set; } = string.Empty;

    [SwaggerSchema("maximum number of players simultaneously allowed on the server")]
    public int? MaxPlayers { get; set; }

    [SwaggerSchema("current number of players")]
    public int? NumPlayers { get; set; }

    [SwaggerSchema("type of game: capture the flag, deathmatch, assault and more")]
    public string GameType { get; set; } = string.Empty;

    [SwaggerSchema("score team terrorists")]
    public int? ScoreTerrorists { get; set; }

    [SwaggerSchema("score team special forces")]
    public int? ScoreSpecialForces { get; set; }

    [SwaggerSchema("round number of the game")]
    public int? RoundNumber { get; set; }

    [SwaggerSchema("last winning team of the round")]
    public int? LastWinningTeam { get; set; }

    [SwaggerSchema("comma-separated mutator/mod list")]
    public string Mutators { get; set; } = string.Empty;

    [SwaggerSchema("time limit per match")]
    public int? TimeLimit { get; set; }

    [SwaggerSchema("friendly fire rate")]
    public string FriendlyFire { get; set; } = string.Empty;

    [SwaggerSchema("tost version")]
    public string TostVersion { get; set; } = string.Empty;

    [SwaggerSchema("server administrator's name")]
    public string AdminName { get; set; } = string.Empty;

    [SwaggerSchema("server administrator's contact information")]
    public string AdminEmail { get; set; } = string.Empty;

    [SwaggerSchema("does the server have a password?")]
    public string Password { get; set; } = string.Empty;
}
