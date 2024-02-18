using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using Chirp.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Chirp.WebService.Controllers
{
    public class UserController : BaseController
    {
        private readonly IAuthorRepository _authorRepository;
        public UserController(ChirpDbContext chirpDbContext, IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository) : base(authorRepository, cheepRepository, likeRepository, commentRepository)
        {
            _authorRepository = authorRepository;
        }

        // POST: User/Register
        [HttpPost]
        [Route("User/Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(IFormCollection collection)
        {
            string? username = collection["username"];
            string? email = collection["email"];
            string? password = collection["password"];
            string? password2 = collection["password2"];

            if (String.IsNullOrEmpty(username))
            {
                return RedirectWithError("You have to enter a username");
            }
            else if (String.IsNullOrEmpty(email))
            {
                return RedirectWithError("Invalid input");
            }
            else if (String.IsNullOrEmpty(password))
            {
                return RedirectWithError("You have to enter a password");
            }
            else if (String.IsNullOrEmpty(password2))
            {
                return RedirectWithError("Invalid input");
            }

            var authorWithUsernameExists = await _authorRepository.AuthorWithUsernameExists(username);

            if ((bool)authorWithUsernameExists)
            {
                return RedirectWithError("The username is already taken");
            }

            if (password != password2)
            {
                return RedirectWithError("The two passwords do not match");
            }

            var user = new AuthorDto { Id = new Guid(), Username = username, Email = email, Password = password };

            await AuthorRepository.AddAuthor(user);

            return Redirect(GetPathUrl());
        }

        // POST: User/Login
        [HttpPost]
        [Route("User/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IFormCollection collection)
        {
            string? username = collection["username"];
            string? password = collection["password"];

            if (String.IsNullOrEmpty(username))
            {
                return RedirectWithError("You have to enter a username");
            }
            else if (String.IsNullOrEmpty(password))
            {
                return RedirectWithError("You have to enter a password");
            }


            var authorWithUsernameExists = await _authorRepository.AuthorWithUsernameExists(username);

            if ((bool)!authorWithUsernameExists)
            {
                return RedirectWithError("Invalid username");
            }

            var user = await _authorRepository.GetAuthorByName(username);

            if (user.Password != password)
            {
                return RedirectWithError("Invalid password");
            }
            else
            {
                // Store user ID in the session
                HttpContext.Session.Set("UserId", user.Id.ToByteArray()); // Assuming Id is of type int, adjust accordingly
            }

            return Redirect("/");
        }

        [HttpPost]
        [Route("User/Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(IFormCollection collection)
        {
            HttpContext.Session.Remove("UserId");

            return Redirect("/");
        }

        [HttpPost]
        [Route("User/DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IFormCollection collection)
        {
            AuthorDto? user = (AuthorDto?)HttpContext.Items["user"];
            if (user is not null)
            {
                
            }

            //This part is weird, doesnt do anything

            await _authorRepository.DeleteAuthor(user.Id);
            Console.WriteLine("----egjierge----");

            HttpContext.Session.Remove("UserId");
            return Redirect("/");
        }
    }
}