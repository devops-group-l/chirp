using Chirp.Core.Dto;
using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CheepController : BaseController
    {
        private readonly ILogger<CheepController> _logger;

        public CheepController(
            IAuthorRepository authorRepository,
            ICheepRepository cheepRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            ISimulationRepository simulationRepository,
            ILogger<CheepController> logger
        )
            : base(
                authorRepository,
                cheepRepository,
                likeRepository,
                commentRepository,
                simulationRepository
            )
        {
            _logger = logger;
        }

        // POST: Cheep/Like
        [HttpPost]
        [Route("Cheep/Like")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(IFormCollection collection)
        {
            _logger.LogInformation("Controller.Like was called");
            return await WithAuthAsync(async user =>
            {
                String? cheepId = collection["cheepId"];
                if (String.IsNullOrEmpty(cheepId))
                {
                    return RedirectWithError("Invalid input");
                }
                Guid cId = Guid.Parse(cheepId);
                await LikeRepository.LikeCheep(user.Id, cId);

                return Redirect(GetPathUrl());
            });
        }

        //POST: Unlike cheep
        [HttpPost]
        [Route("Cheep/Unlike")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(IFormCollection collection)
        {
            _logger.LogInformation("Controller.Unlike was called");
            return await WithAuthAsync(async user =>
            {
                String? cheepId = collection["cheepId"];
                if (String.IsNullOrEmpty(cheepId))
                {
                    return RedirectWithError("Invalid input");
                }
                Guid cId = Guid.Parse(cheepId);
                await LikeRepository.UnlikeCheep(user.Id, cId);

                return Redirect(GetPathUrl());
            });
        }

        // POST: Cheep/Create
        [HttpPost]
        [Route("Cheep/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            _logger.LogInformation("Controller.Create was called");
            return await WithAuthAsync(async user =>
            {
                string? cheepText = collection["cheepText"];

                if (String.IsNullOrEmpty(cheepText))
                {
                    return RedirectWithError("Invalid input");
                }

                if (cheepText.Length > 160)
                {
                    return RedirectWithError(
                        "Invalid input - cheep is too long (max 160 characters)"
                    );
                }

                await CheepRepository.AddCheep(
                    new AddCheepDto { AuthorId = user.Id, Text = cheepText }
                );
                return Redirect(GetPathUrl());
            });
        }

        // POST: Cheep/Delete
        [HttpPost]
        [Route("Cheep/Delete")]
        [ValidateAntiForgeryToken]
        //public IActionResult Delete(Guid id)
        public async Task<IActionResult> Delete(IFormCollection collection)
        {
            _logger.LogInformation("Controller.Delete was called");
            return await WithAuthAsync(async _ =>
            {
                string? cheepId = collection["cheepId"];

                if (String.IsNullOrEmpty(cheepId))
                {
                    _logger.LogDebug(
                        "Controller.Delete was called and the Cheep Id was invalid, cheepId: "
                            + cheepId
                    );
                    return RedirectWithError("Invalid Cheep Id");
                }

                if (!await CheepRepository.DeleteCheep(Guid.Parse(cheepId)))
                    return RedirectWithError("Cheep was not found");

                return Redirect(GetPathUrl());
            });
        }

        //Post Cheep/Follow
        [HttpPost]
        [Route("Cheep/Follow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                //The new account to follow
                Guid authorToBeFollowed = Guid.Parse(collection["CheepAuthorId"].ToString());

                _logger.LogInformation(
                    "Controller.Follow was called with user: {user.Id} try to follow user: {authorToBeFollowed}",
                    user.Id,
                    authorToBeFollowed
                );

                await AuthorRepository.AddFollow(user.Id, authorToBeFollowed);

                return Redirect(GetPathUrl()); //Redirect to same page
            });
        }

        //Post Cheep/Unfollow
        [HttpPost]
        [Route("Cheep/Unfollow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(IFormCollection collection)
        {
            return await WithAuthAsync(async user =>
            {
                Guid authorToBeUnfollowed = Guid.Parse(collection["CheepAuthorId"].ToString()); //The new account to follow

                _logger.LogInformation(
                    "Controller.UnFollow was called with user: {user.Id} try to unfollow user: {authorToBeUnfollowed}",
                    user.Id,
                    authorToBeUnfollowed
                );

                await AuthorRepository.RemoveFollow(user.Id, authorToBeUnfollowed);

                return Redirect(GetPathUrl()); //Redirect to same page
            });
        }
    }
}
