namespace Chirp.Infrastructure.Models;

public class SimulationFollows
{
    public int? Id { get; set; }
    public string Follower { get; set; } = null!;
    public string Follows { get; set; } = null!;
}