using Chirp.Core.Repositories;
using Chirp.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CommentController : BaseController
    {
        private readonly ILogger<CommentController> _logger;
        public CommentController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, ISimulationRepository simulationRepository, ILogger<CommentController> logger) : base(authorRepository, cheepRepository, likeRepository, commentRepository, simulationRepository)
        {
            _logger = logger;
        }
        
        //POST: Comment/Add
        [HttpPost]
        [Route("Comment/Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(IFormCollection collection)
        {
            _logger.LogInformation("CommentController.Add was called");
            return await WithAuthAsync(async _ =>
            {
                String commentString = collection["CommentText"].ToString();
                Guid cheepId = Guid.Parse(collection["CheepId"].ToString());
                Guid authorId = Guid.Parse(collection["AuthorId"].ToString());
                
                AddCommentDto addCommentDto = new AddCommentDto
                {
                    AuthorId = authorId,
                    CheepId = cheepId,
                    Text = commentString
                };
                
                await CommentRepository.AddComment(addCommentDto);
                
                return Redirect(GetPathUrl());
            });
        }
        
        //POST: Comment/Remove
        [HttpPost]
        [Route("Comment/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IFormCollection collection)
        {
            _logger.LogInformation("CommentController.Delete was called");
            return await WithAuthAsync(async _ =>
            {
                Guid commentId = Guid.Parse(collection["CommentId"].ToString());
                await CommentRepository.DeleteComment(commentId);

                return Redirect(GetPathUrl());
            });
        }
    }
}