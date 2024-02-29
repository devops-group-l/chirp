using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Chirp.WebService.Controllers
{
    public class UserController : BaseController
    {
        private readonly IAuthorRepository _authorRepository;
        public UserController(ChirpDbContext chirpDbContext, IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, ISimulationRepository simulationRepository) : base(authorRepository, cheepRepository, likeRepository, commentRepository, simulationRepository)
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

            string hashedPassword = SimpleHashPassword(password);

            var user = new AuthorDto { Id = new Guid() , Username = username, Email = email, Password = hashedPassword };

            await AuthorRepository.AddAuthor(user);

            return Redirect("/Login");
        }

        // POST: User/Login
        [HttpPost]
        [Route("User/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IFormCollection collection)
        {
            string? username = collection["username"];
            string? enteredPassword = collection["password"];

            string password = SimpleHashPassword(enteredPassword);

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
                HttpContext.Session.Set("UserId", user.Id.ToByteArray()); 
            }

            return Redirect("/");
        }

        [HttpPost]
        [Route("User/Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserId");

            return Redirect("/");
        }

        [HttpPost]
        [Route("User/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            AuthorDto? user = (AuthorDto?)HttpContext.Items["user"];
            if (user is null) {
                return RedirectWithError("No users found to be deleted!");
            }
            await _authorRepository.DeleteAuthor(user.Id);

            HttpContext.Session.Remove("UserId");
            
            return Redirect("/");
        }

        static string SimpleHashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}