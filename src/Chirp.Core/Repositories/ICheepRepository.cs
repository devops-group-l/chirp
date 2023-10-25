namespace Chirp.Core.Repositories;
using Dto;

public interface ICheepRepository
{
    public CheepDto AddCheep(AddCheepDto cheep);
    public int GetCheepCount();
    public int GetAuthorCheepCount(string authorName);
    public List<CheepDto> GetCheepsForPage(int pageNumber);
    public List<CheepDto> GetAuthorCheepsForPage(string authorName, int pageNumber);
}