using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly ISimulationRepository _simulationRepository;
    
    public List<CheepPartialModel> Cheeps { get; set; } = new ();
    public FooterPartialModel FooterPartialModel { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository, ISimulationRepository simulationRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
        _simulationRepository = simulationRepository;
    }
    
    public async Task<IActionResult> OnGet(string author)
    {
        AuthorDto? user = (AuthorDto?)HttpContext.Items["user"];
        // Get amount of pages
        int amountOfPages;
        if (user is not null)
        {
            amountOfPages = (int)Math.Ceiling((double)await _simulationRepository.GetAuthorCheepCount(author) / 32);
        }
        else
        {
            amountOfPages = (int)Math.Ceiling((double)await _simulationRepository.GetAuthorCheepCount(author) / 32);
        }
        
        // Get page number
        int pageNumber = 1;
        if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int pageParameter))
        {
            //If parameter is too large -> set to max
            pageNumber = (pageParameter > amountOfPages) ? amountOfPages : pageParameter;
        }

        // Get cheep dtos
        List<SimulationMessageDto> cheepDtos;
        
        cheepDtos = await _simulationRepository.GetAuthorCheepsForPage(author, pageNumber);
        Console.WriteLine("Break for investigation");
        /*
        if (user is null)
        {
            cheepDtos = await _simulationRepository.GetAuthorCheepsForPage(author, pageNumber);
        }
        else
        {
            if (user.Username.Equals(author))
            {
                cheepDtos = await _simulationRepository.GetAuthorCheepsForPageAsOwner(user.Id, pageNumber);
            }
            else
            {
                cheepDtos = await _simulationRepository.GetAuthorCheepsForPage(author, pageNumber);
            }
        }
        */
        
        // Build models
        var cheepPartialModels = new List<CheepPartialModel>();

        List<string>? follows = null;
                List<LikeDto>? likes = null;
                
                //If the user is not null -> update the list of follows and likes
                /*
                if (user is not null)
                {
                    follows = await _authorRepository.GetFollowsForAuthor(user.Id);
                    likes = await _likeRepository.GetLikesByAuthorId(user.Id);
                }
                */
                
                //Generate a cheep model for each CheepDto on page
                foreach (SimulationMessageDto messageDto in cheepDtos)
                {
                    cheepPartialModels.Add(CheepPartialModel.BuildCheepPartialModelFromSimulation(messageDto));
                }

        Cheeps = cheepPartialModels;

        FooterPartialModel = FooterPartialModel.BuildFooterPartialModel(pageNumber, amountOfPages, author);
        
        return Page();
    }
}