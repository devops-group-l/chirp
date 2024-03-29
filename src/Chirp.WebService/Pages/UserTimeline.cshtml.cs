﻿using Chirp.Core.Dto;
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
    
    public List<CheepPartialModel> Cheeps { get; set; } = new ();
    public FooterPartialModel FooterPartialModel { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, ILikeRepository likeRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _likeRepository = likeRepository;
    }
    
    public async Task<IActionResult> OnGet(string author)
    {
        AuthorDto? user = (AuthorDto?)HttpContext.Items["user"];
        // Get amount of pages
        int amountOfPages;
        if (user is not null)
        {
            amountOfPages = (int)Math.Ceiling((double)await _cheepRepository.GetAuthorCheepCount(author, user.Id) / 32);
        }
        else
        {
            amountOfPages = (int)Math.Ceiling((double)await _cheepRepository.GetAuthorCheepCount(author) / 32);
        }
        
        // Get page number
        int pageNumber = 1;
        if (Request.Query.ContainsKey("page") && int.TryParse(Request.Query["page"], out int pageParameter))
        {
            //If parameter is too large -> set to max
            pageNumber = (pageParameter > amountOfPages) ? amountOfPages : pageParameter;
        }

        // Get cheep dtos
        List<CheepDto> cheepDtos;
        
        if (user is null)
        {
            cheepDtos = await _cheepRepository.GetAuthorCheepsForPage(author, pageNumber);
        }
        else
        {
            if (user.Username.Equals(author))
            {
                cheepDtos = await _cheepRepository.GetAuthorCheepsForPageAsOwner(user.Id, pageNumber);
            }
            else
            {
                cheepDtos = await _cheepRepository.GetAuthorCheepsForPage(author, pageNumber);
            }
        }
        
        // Build models
        var cheepPartialModels = new List<CheepPartialModel>();

        List<string>? follows = null;
                List<LikeDto>? likes = null;
                
                //If the user is not null -> update the list of follows and likes
                if (user is not null)
                {
                    follows = await _authorRepository.GetFollowsForAuthor(user.Id);
                    likes = await _likeRepository.GetLikesByAuthorId(user.Id);
                }
                
                //Generate a cheep model for each CheepDto on page
                foreach (CheepDto cheepDto in cheepDtos)
                {
                    cheepPartialModels.Add(CheepPartialModel.BuildCheepPartialModel(cheepDto, likes, follows));
                }

        Cheeps = cheepPartialModels;

        FooterPartialModel = FooterPartialModel.BuildFooterPartialModel(pageNumber, amountOfPages, author);
        
        return Page();
    }
}