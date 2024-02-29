using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

public class SimulationMessage
{
    [Key]
    public int Id { get; set; }
    public string? text { get; set; }
    public string? pub_date { get; set; }
    public string? username { get; set; }
    
}