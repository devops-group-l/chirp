using System.Security.Cryptography;
using System.Text;
using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.WebService.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class UserController : BaseController
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IAuthorRepository authorRepository,
            ICheepRepository cheepRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            ISimulationRepository simulationRepository,
            ILogger<UserController> logger
        )
            : base(
                authorRepository,
                cheepRepository,
                likeRepository,
                commentRepository,
                simulationRepository
            )
        {
            _authorRepository = authorRepository;
            _logger = logger;
        }

        // POST: User/Register
        [HttpPost]
        [Route("User/Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(IFormCollection collection)
        {
            _logger.LogInformation("UserController.Register was called");
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

            var authorWithUsernameExists = await _authorRepository.AuthorWithUsernameExists(
                username
            );

            if ((bool)authorWithUsernameExists)
            {
                return RedirectWithError("The username is already taken");
            }

            if (password != password2)
            {
                return RedirectWithError("The two passwords do not match");
            }

            string hashedPassword = password.SimpleHashPassword();

            var user = new AuthorDto
            {
                Id = new Guid(),
                Username = username,
                Email = email,
                Password = hashedPassword
            };

            await AuthorRepository.AddAuthor(user);

            return Redirect("/Login");
        }

        // POST: User/Login
        [HttpPost]
        [Route("User/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IFormCollection collection)
        {
            _logger.LogInformation("UserController.Login was called");
            string? username = collection["username"];
            string? enteredPassword = collection["password"];

            string password = enteredPassword?.SimpleHashPassword() ?? "";

            if (String.IsNullOrEmpty(username))
            {
                return RedirectWithError("You have to enter a username");
            }
            else if (String.IsNullOrEmpty(password))
            {
                return RedirectWithError("You have to enter a password");
            }

            var authorWithUsernameExists = await _authorRepository.AuthorWithUsernameExists(
                username
            );

            if ((bool)!authorWithUsernameExists)
            {
                return RedirectWithError("Invalid username");
            }

            var user = await _authorRepository.GetAuthorByName(username);

            if (user.Password != password)
            {
                return RedirectWithError("Invalid password " + user.Password + " - " + password);
            }
            else
            {
                // Store user ID in the session
                HttpContext.Session.Set("UserId", user.Id.ToByteArray());
            }

            return Redirect("/");
        }

        [HttpPost]
        [Route("User/Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("UserController.Logout was called");

            HttpContext.Session.Remove("UserId");

            return Redirect("/");
        }

        [HttpPost]
        [Route("User/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            _logger.LogInformation("UserController.Delete was called");
            Guid? userId = (Guid?)HttpContext.Items["userId"];
            if (userId is null)
            {
                _logger.LogDebug(
                    "UserController.Delete was called, but no user was found on ID: {userId}",
                    userId
                );
                return RedirectWithError("No users found to be deleted!");
            }
            await _authorRepository.DeleteAuthor(userId.Value);

            HttpContext.Session.Remove("UserId");

            return Redirect("/");
        }
    }
}
