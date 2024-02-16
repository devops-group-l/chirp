using Chirp.Core.Dto;
using Chirp.Core.Extensions;
using Chirp.Core.Repositories;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers;

public abstract class BaseController : Controller, IController
{
    public virtual Func<ClaimsUser?> GetUser { get; }
    public virtual Func<string> GetPathUrl { get; }
    
    protected readonly IAuthorRepository AuthorRepository;
    protected readonly ICheepRepository CheepRepository;
    protected readonly ILikeRepository LikeRepository;
    protected readonly ICommentRepository CommentRepository;
    
    protected BaseController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository)
    {
        AuthorRepository = authorRepository;
        CheepRepository = cheepRepository;
        CommentRepository = commentRepository;
        
        GetUser = () => User.GetUser();
        LikeRepository = likeRepository;
        GetUser = () => User.GetUser();
        GetPathUrl = () => Request.GetPathUrl();
    }
    
    protected async Task<IActionResult> WithAuthAsync(Func<ClaimsUser, Task<IActionResult>> protectedFunction)
    {
        try
        {
            var rawuser = GetUser();
            if (rawuser is null) throw new ArgumentException("User is null in Auth async");
            var user = rawuser.GetUserNonNull();
            
            await AuthorRepository.AddAuthor(new AuthorDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl
            });

            return await protectedFunction(user);
        }
        catch (ArgumentException)
        {
            return Unauthorized();
        }
        catch
        {
            return BadRequest("Unknown Error Occurred");
        }
    }

    // POST: Cheep/Create
    [HttpPost]
    [Route("User/Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterUser(IFormCollection collection)
    {
        return await WithAuthAsync(async user =>
        {
            string? cheepText = collection["cheepText"];

            if (String.IsNullOrEmpty(cheepText))
            {
                return RedirectWithError("Invalid input");
            }

            if (cheepText.Length > 160)
            {
                return RedirectWithError("Invalid input - cheep is too long (max 160 characters)");
            }

            await CheepRepository.AddCheep(new AddCheepDto
            {
                AuthorId = user.Id,
                Text = cheepText
            });
            return Redirect(GetPathUrl());
        });
    }                   

    public ActionResult RedirectWithError(string errorMessage)
    {
        var pathUrl = GetPathUrl();
        if (pathUrl.Contains("errorMessage=")) return Redirect(pathUrl);
        
        var queryDelimiter = pathUrl.Contains('?') ? "&" : "?";
        return Redirect(GetPathUrl() + $"{queryDelimiter}errorMessage={errorMessage}");
    }
}