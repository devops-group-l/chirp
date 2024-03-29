namespace Chirp.Core.Dto;

public class AuthorDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
    public string Password { get; init; }
    public string AvatarUrl { get; init; }
}