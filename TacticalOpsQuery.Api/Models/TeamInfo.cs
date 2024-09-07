using Swashbuckle.AspNetCore.Annotations;

namespace TacticalOpsQuery.Api.Models;

[SwaggerSchema(Description = "Detailed team information")]
public class TeamInfo
{
    [SwaggerSchema("team display name")]
    public string Name { get; set; } = string.Empty;

    [SwaggerSchema("score team")]
    public int Score { get; set; }

    [SwaggerSchema(Description = "Detailed players information")]
    public List<Player> Players { get; set; } = new List<Player>();
}
