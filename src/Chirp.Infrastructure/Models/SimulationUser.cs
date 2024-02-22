using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class SimulationUser
{
    [Key]
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PwdHashed { get; set; }
}