using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface ISimulationRepository
{
    public Task AddUser(SimulationUserDto sud);

    public Task<List<SimulationMessageDto>> GetMessages(int amount);

    public Task<List<SimulationMessageDto>> GetSpecificMessages(string username, int amount);

    public void AddMessage(SimulationMessageDto message);
}