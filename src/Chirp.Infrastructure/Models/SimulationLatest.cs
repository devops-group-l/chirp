using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class SimulationLatest
{
    [Key]
    public int Id { get; set; }

    public int LatestValue { get; set; } = -1;
}
