using Swashbuckle.AspNetCore.Annotations;

namespace TacticalOpsQuery.Api.Models;

[SwaggerSchema(Description = "Detailed player information")]
public class Player
{
    [SwaggerSchema("player display name")]
    public string Name { get; set; } = string.Empty;

    [SwaggerSchema("number of kills")]
    public int? Frags { get; set; }

    [SwaggerSchema("number of deaths")]
    public int? Deaths { get; set; }

    [SwaggerSchema("number of score")]
    public int? Score { get; set; }

    [SwaggerSchema("player ping")]
    public int? Ping { get; set; }

    [SwaggerSchema("player indication as team number")]
    public int? Team { get; set; }

    [SwaggerSchema("player model / mesh")]
    public string Mesh { get; set; } = string.Empty;

    [SwaggerSchema("player body texture")]
    public string Skin { get; set; } = string.Empty;

    [SwaggerSchema("player display name")]
    public int? Health { get; set; }
}
