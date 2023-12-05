using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

/*Like repository to implement likes in cheeps*/

public class LikeRepository : ILikeRepository
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    private readonly ChirpDbContext _chirpDbContext;

    public LikeRepository(ChirpDbContext chirpDbContext, IAuthorRepository authorRepository, ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _chirpDbContext = chirpDbContext;
        _chirpDbContext.Database.EnsureCreated();
        
    }

    public void LikeCheep(Guid authorId, Guid cheepId)
    {
        if (IsLiked(authorId, cheepId))
        {
            return;
        }

        _chirpDbContext.Likes.Add(new Like
        {
            LikedByAuthorId = authorId,
            CheepId = cheepId
        });
        _chirpDbContext.SaveChanges();
    }

    public void UnlikeCheep(Guid authorId, Guid cheepId)
    {
        if (IsLiked(authorId, cheepId))
        {
            var toUnlike = _chirpDbContext.Likes.First(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
            _chirpDbContext.Remove(toUnlike);
            _chirpDbContext.SaveChanges();
        }
    }

    public int LikeCount(Guid cheepId)
    {
        return _chirpDbContext.Likes.Count(x => x.CheepId == cheepId);
    }

    public List<LikeDto> GetLikesByAuthorId(Guid authorId)
    {
        return _chirpDbContext.Likes
            .Where(l => l.LikedByAuthorId == authorId)
            .Select<Like, LikeDto>(l =>
                new LikeDto
                {
                    CheepId = l.CheepId,
                    LikedByAuthorId = l.LikedByAuthorId
                }
            ).ToList();
    }
   

    public List<LikeDto> GetLikesByCheepId(Guid cheepId)
    {
        return _chirpDbContext.Likes
            .Where(l => l.CheepId == cheepId)
            .Select<Like, LikeDto>(l =>
                new LikeDto
                {
                    CheepId = l.CheepId,
                    LikedByAuthorId = l.LikedByAuthorId
                }
            ).ToList();
    }

    public bool IsLiked(Guid authorId, Guid cheepId)
    {
        if (!_chirpDbContext.Likes.Any(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId))
        {
            return false;
        } 
        return _chirpDbContext.Likes.Any(x => x.LikedByAuthorId == authorId && x.CheepId == cheepId);
    }

}