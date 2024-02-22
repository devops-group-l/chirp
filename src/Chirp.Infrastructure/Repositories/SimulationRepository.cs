using Chirp.Core.Dto;
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
            Username = sud.Username,
            Email = sud.Email,
            PwdHashed = sud.PwdHashed
        };

        _chirpDbContext.SimulationUsers.Add(user);
        await _chirpDbContext.SaveChangesAsync();
    }

    public async Task<List<SimulationMessageDto>> GetMessages(int amountToRetrieve)
    {
        List<SimulationMessageDto> dtoList = await _chirpDbContext.SimulationMessages.Select(p => 
            new SimulationMessageDto
            {
                username = p.username,
                text = p.text,
                pub_date = p.pub_date
            }).ToListAsync();
            
        return dtoList.Take(amountToRetrieve).ToList();
    }

    public async Task<List<SimulationMessageDto>> GetSpecificMessages(string username, int amount)
    {
        List<SimulationMessageDto> dtoList = await _chirpDbContext.SimulationMessages
            .Where(sm => sm.username == username).Select(p =>
                new SimulationMessageDto
                {
                    username = p.username,
                    text = p.text,
                    pub_date = p.pub_date
                }).OrderByDescending(x => x.pub_date).ToListAsync();

        return dtoList.Take(amount).ToList();
    }

    public void AddMessage(SimulationMessageDto message)
    {
        _chirpDbContext.SimulationMessages.Add(new SimulationMessage
        {
            pub_date = message.pub_date,
            text = message.text,
            username = message.username
        });

        _chirpDbContext.SaveChanges();
    }
}