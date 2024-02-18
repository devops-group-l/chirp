using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure.Models;

// Principal (parent)
public class Author
{
    [Key]
    public Guid AuthorId { get; set; }

    //https://stackoverflow.com/questions/53815532/dataannotations-for-valid-email-address
    [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
    public string Email { get; set; }
    
    [
        MaxLength(40, ErrorMessage = "Username must contain less than 40 characters"),
        MinLength(6, ErrorMessage = "Username must contain more than 6 characters")
    ]
    public required string Username { get; set; }

    [
        MaxLength(300, ErrorMessage = "URL must contain less than 300 characters"), 
        MinLength(5, ErrorMessage = "URL must contain more than 5 characters")
    ]
    public required string Password { get; set; }
    public string AvatarUrl { get; set; }
    
    public ICollection<Cheep> Cheeps { get; } = new List<Cheep>();
    public ICollection<Comment> Comments { get; } = new List<Comment>();
    public ICollection<Author> Follows { get; } = new List<Author>();
    public ICollection<Author> FollowedBy { get; } = new List<Author>();
    public ICollection<Like> Likes { get; } = new List<Like>();
}