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
    public const string SessionKeyName = "_UserId";
    
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
            string? username = collection["username"];

            if (String.IsNullOrEmpty(username))
            {
                return RedirectWithError("Invalid input");
            }

            string? email = collection["email"];

            if (String.IsNullOrEmpty(email))
            {
                return RedirectWithError("Invalid input");
            }
            // else if (metode der ser om email findes i db){
            //              
            // }

            string? password = collection["password"];

            if (String.IsNullOrEmpty(password)){
                return RedirectWithError("Invalid input");
            }

            string? password2 = collection["password2"];

            if (String.IsNullOrEmpty(password2)){
                return RedirectWithError("Invalid input");
            } else if (password != password2){
                return RedirectWithError("Password does not match");
            }

            var userId = new Guid();
            context.Session.Set("user_id", userId.ToByteArray()); // sets the user_id var
            

            await AuthorRepository.AddAuthor(new AuthorDto
            {
                Id = userId,
                Name = user.Name,
                Username = username,
                AvatarUrl = "https://gravatar.com/avatar/d2f178641c50d4ea8127b8c8fd99c99c?s=400&d=identicon&r=x"
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