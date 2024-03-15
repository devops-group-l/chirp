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
    protected readonly ISimulationRepository SimulationRepository;
    
    protected BaseController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, ISimulationRepository simulationRepository)
    {
        AuthorRepository = authorRepository;
        CheepRepository = cheepRepository;
        CommentRepository = commentRepository;
        SimulationRepository = simulationRepository;
        
        GetUser = () => User.GetUser();
        LikeRepository = likeRepository;
        GetUser = () => User.GetUser();
        GetPathUrl = () => Request.GetPathUrl();
    }
    
    protected async Task<IActionResult> WithAuthAsync(Func<AuthorDto, Task<IActionResult>> protectedFunction)
    {
        try
        {
            Guid? rawUserId = (Guid?)HttpContext.Items["userId"];
            if (rawUserId is null) throw new ArgumentException("User is null in Auth async");
            // var user = rawUserId;

            var user = await AuthorRepository.GetAuthorById(rawUserId.Value);

            if (user is null) return RedirectWithError("UserNotFound");
            
            await AuthorRepository.AddAuthor(new AuthorDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
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

    public ActionResult RedirectWithError(string errorMessage)
    {
        var pathUrl = GetPathUrl();
        if (pathUrl.Contains("errorMessage=")) return Redirect(pathUrl);
        
        var queryDelimiter = pathUrl.Contains('?') ? "&" : "?";
        return Redirect(GetPathUrl() + $"{queryDelimiter}errorMessage={errorMessage}");
    }
}