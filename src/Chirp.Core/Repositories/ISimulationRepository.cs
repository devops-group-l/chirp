using Chirp.Core.Dto;

namespace Chirp.Core.Repositories;

public interface ISimulationRepository
{
    public Task AddUser(SimulationUserDto sud);

    public Task<List<SimulationMessageDto>> GetMessages(int amount);

    public Task<List<SimulationMessageDto>> GetMessagesSorted(int pageNumber);

    public Task<List<SimulationMessageDto>> GetSpecificMessages(string username, int amount);

    public Task AddMessage(SimulationMessageDto message);

    public List<string> GetFollowsForUser(string username, int amount);

    public Task AddFollower(string follower, string following);

    public void RemoveFollower(string follower, string following);

    public Boolean CheckIfUserExists(string username);
}