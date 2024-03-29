﻿using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class SimulationRepository : ISimulationRepository
{
    private readonly ChirpDbContext _chirpDbContext;

    public SimulationRepository(ChirpDbContext chirpDbContext)
    {
        _chirpDbContext = chirpDbContext;
    }

    public async Task AddUser(SimulationUserDto sud)
    {
        SimulationUser user = new SimulationUser
        {
            Username = sud.Username.Trim(),
            Email = sud.Email.Trim(),
            PwdHashed = sud.PwdHashed.Trim()
        };

        await _chirpDbContext.SimulationUsers.AddAsync(user);
        await _chirpDbContext.SaveChangesAsync();
    }

    public async Task<List<SimulationMessageDto>> GetMessages(int amountToRetrieve)
    {
        List<SimulationMessageDto> dtoList = await _chirpDbContext.SimulationMessages.Select(p => 
            new SimulationMessageDto
            {
                username = p.username.Trim(),
                text = p.text,
                pub_date = p.pub_date
            }).ToListAsync();
            
        return dtoList.Take(amountToRetrieve).ToList();
    }

    public async Task<List<SimulationMessageDto>> GetSpecificMessages(string username, int amount)
    {
        List<SimulationMessageDto> dtoList = await _chirpDbContext.SimulationMessages
            .Where(sm => sm.username == username.Trim()).Select(p =>
                new SimulationMessageDto
                {
                    username = p.username.Trim(),
                    text = p.text,
                    pub_date = p.pub_date
                }).OrderByDescending(x => x.pub_date).ToListAsync();

        return dtoList.Take(amount).ToList();
    }

    public async Task AddMessage(SimulationMessageDto message)
    {
        await _chirpDbContext.SimulationMessages.AddAsync(new SimulationMessage
        {
            pub_date = message.pub_date,
            text = message.text,
            username = message.username.Trim()
        });

        await _chirpDbContext.SaveChangesAsync();
    }

    public List<string> GetFollowsForUser(string username, int amount)
    {
        var query = _chirpDbContext.SimulationFollows.Where(p => p.Follower == username.Trim()).Select(p => p.Follows)
            .Take(amount).ToList();

        return query;
    }

    public async Task AddFollower(string follower, string following)
    {
        await _chirpDbContext.SimulationFollows.AddAsync(new SimulationFollows()
        {
            Follower = follower.Trim(),
            Follows = following.Trim()
        });

        await _chirpDbContext.SaveChangesAsync();
    }

    public void RemoveFollower(string follower, string following)
    {
        SimulationFollows? follow =
            _chirpDbContext.SimulationFollows.FirstOrDefault(p => p.Follower == follower.Trim() && p.Follows == following.Trim());

        if (follow != null) _chirpDbContext.SimulationFollows.Remove(follow);

        _chirpDbContext.SaveChanges();
    }

    public Boolean CheckIfUserExists(string username)
    {
        return _chirpDbContext.SimulationUsers.AsNoTracking().Any(u => u.Username == username.Trim());
    }
}