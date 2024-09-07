using Swashbuckle.AspNetCore.Annotations;

namespace TacticalOpsQuery.Api.Models;

[SwaggerSchema(Description = "Detailed status information")]
public class ServerStatus
{
    [SwaggerSchema(Description = "Detailed server information")]
    public ServerInfo? ServerInfo { get; set; }

    [SwaggerSchema(Description = "Detailed player information")]
    public List<Player>? Players { get; set; }
}
