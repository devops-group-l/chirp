using Chirp.Core.Dto;

namespace Chirp.WebService.Controllers;

public interface IController
{
    public Func<AuthorDto?> GetUser { get; }
    public Func<string> GetPathUrl { get; }
}
