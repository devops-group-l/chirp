using System.Globalization;
using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class SimulationRepository : ISimulationRepository
{
    private readonly ChirpDbContext _chirpDbContext;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;

    public SimulationRepository(ChirpDbContext chirpDbContext, IAuthorRepository authorRepository, ICheepRepository cheepRepository)
    {
        _chirpDbContext = chirpDbContext;
        _authorRepository = authorRepository;
        _cheepRepository = cheepRepository;
    }

    public async Task AddUser(SimulationUserDto sud)
    {
        var user = new SimulationUser()
        {
            Username = sud.Username.Trim(),
            Email = sud.Email.Trim(),
            PwdHashed = sud.PwdHashed.Trim()
        };

        var dto = new AuthorDto()
        {
            Email = user.Email,
            Id = new Guid(),
            Password = user.PwdHashed,
            Username = user.Username,
        };

        await _authorRepository.AddAuthor(dto);
    }
    
    public async Task<List<SimulationMessageDto>> GetMessages(int amountToRetrieve)
    {
        return await _chirpDbContext
            .Cheeps
            .OrderByDescending(c => c.Timestamp)
            .Take(amountToRetrieve)
            .Select(c =>
                new SimulationMessageDto()
                {
                    username = c.Author.Username.Trim(),
                    text = c.Text,
                    pub_date = c.Timestamp.ToString(CultureInfo.CurrentCulture),
                })
            .ToListAsync();
    }

    public async Task<List<SimulationMessageDto>> GetSpecificMessages(string username, int amount)
    {
        return await _chirpDbContext.Cheeps
            .Where(c => c.Author.Username == username.Trim())
            .Select(c =>
                new SimulationMessageDto
                {
                    username = c.Author.Username.Trim(),
                    text = c.Text,
                    pub_date = c.Timestamp.ToString(CultureInfo.CurrentCulture),
                }).OrderByDescending(x => x.pub_date)
            .Take(amount)
            .ToListAsync();
    }

    public async Task AddMessage(SimulationMessageDto message)
    {
        var author = await _authorRepository.GetAuthorByName(message.username);
            
        await _cheepRepository.AddCheep(new AddCheepDto
        {
            AuthorId = author.Id,
            Text = message.text
        });
    }

    public async Task<List<string>> GetFollowsForUser(string username, int amount)
    {
        var author = await _authorRepository.GetAuthorByName(username.Trim());

        var followers = await _authorRepository.GetFollowsForAuthor(author.Id);

        return followers.Take(amount).ToList();
    }

    public async Task AddFollower(string follower, string following)
    {
        AuthorDto? followerDto = await _authorRepository.GetAuthorByName(follower.Trim());
        AuthorDto? followingDto = await _authorRepository.GetAuthorByName(following.Trim());

        await _authorRepository.AddFollow(followingDto.Id, followerDto.Id);
    }

    public async Task RemoveFollower(string follower, string following)
    {
        AuthorDto? followerDto = await _authorRepository.GetAuthorByName(follower.Trim());
        AuthorDto? followingDto = await _authorRepository.GetAuthorByName(following.Trim());

        await _authorRepository.AddFollow(followingDto.Id, followerDto.Id);
    }

    public Boolean CheckIfUserExists(string username)
    {
        return _chirpDbContext.SimulationUsers.AsNoTracking().Any(u => u.Username == username.Trim());
    }

    public async Task UpdateLatest(int value)
    {
        var latest = await _chirpDbContext.SimulationLatests.FirstOrDefaultAsync();

        if (latest == null)
        {
            //This should happen the first the database is initialized, as there does not exist a "latest" yet
            var newLatest = new SimulationLatest
            {
                LatestValue = value
            };
            await _chirpDbContext.SimulationLatests.AddAsync(newLatest);
            return;
        }

        latest.LatestValue = value;

        await _chirpDbContext.SaveChangesAsync();
    }

    public async Task<int> GetLatest()
    {
        var latest = await _chirpDbContext.SimulationLatests.FirstOrDefaultAsync();

        if (latest == null) return -1;

        return latest.LatestValue;
    }
}