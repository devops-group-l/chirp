using Chirp.Core.Repositories;
using Chirp.WebService.Controllers.SimulationModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace Chirp.WebService.Controllers;
[Route("api/[controller]")]//TODO: CHANGE
[ApiController]
public class SimulationController : BaseController
{
    private readonly string _latestFileLocation = "./SimulationModels/latest_processed_sim_action_id.txt";
    
    public SimulationController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository) : base(authorRepository, cheepRepository, likeRepository, commentRepository)
    {
    }

    [HttpGet("latest")]
    public ActionResult HandleLatest()
    {
        int latestProcessedCommandId = -1;

        try
        {
            string content = System.IO.File.ReadAllText(_latestFileLocation);
            latestProcessedCommandId = int.Parse(content);
        }
        catch (Exception e)
        {
            latestProcessedCommandId = -1;//Fallback
        }

        return Ok(new {latest = latestProcessedCommandId });
    }

    [HttpPost("register")]
    public async Task<ActionResult> HandleRegister(HTTPHandleRegisterModel requestModel)
    {
        update_latest(HttpContext);

        string? error = null;

        if (requestModel.username == null) error = "You have to enter a username";
        else if (requestModel.email == null || requestModel.email.Contains("@"))
            error = "You have to enter a valid email address";
        else if (requestModel.pwd == null) error = "You have to enter a password";
        else if (await user_id_exists(requestModel.username)) error = "The username is already taken";
        
        //Passed validation checks -> register user
        //TODO: Implement actual handling of user creation

        if (error != null)
        {
            JObject returnModel = new JObject()
            {
                { "status", 400 },
                { "error_msg", error }
            };

            return BadRequest(returnModel);
        }

        return NoContent();//HTTP 204
    }

    [HttpGet("msgs")]
    public HttpResponseMessage HandleMsgs()
    {
        update_latest(HttpContext);

        bool valid = not_req_from_simulator(HttpContext);

        if (!valid)
        {
            string error = "You are not authorized to use this resource!";
            JObject returnObject = new JObject
            {
                { "status", 403 },
                { "error_msg", error }
            };
            
            //Could not find built in .NET 403 status with message, so constructed my own
            var rsp = new HttpResponseMessage(HttpStatusCode.Forbidden);
            rsp.Content = new StringContent(returnObject.ToString());
            return rsp;
        }
        
        //TODO: Continue

        return new HttpResponseMessage(HttpStatusCode.Forbidden);//Temp
    }
    
    //TODO: Implement remaining GET/POST for msgs/username
    //TODO: Implement remaining GET/POST for fllws/username
    
    //Helpers functions
    private Boolean not_req_from_simulator(HttpContext request)
    {
        if (!request.Request.Headers.ContainsKey("Authorization")) return true;

        try
        {
            string value = request.Request.Headers["Authorization"].ToString();

            if (!value.Equals("Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")) return true;
        }
        catch (Exception e)
        {
            return true;//Invalid request header
        }
        
        return false;
    }
    
    private void update_latest(HttpContext request)
    {
        string? parsedCommandIdString = request.Request.Query["latest"];
        int parsedCommandId = -1;

        //Convert the query value to an integer
        if (!string.IsNullOrEmpty(parsedCommandIdString))
        {
            if (int.TryParse(parsedCommandIdString, out int intValue)) parsedCommandId = intValue;
        }
        
        //If command is valid -> store in .txt file
        if (parsedCommandId != -1)
        {
            System.IO.File.WriteAllText(_latestFileLocation, parsedCommandId.ToString());
        }
    }

    //Same functionality as "get_user_id" from API Specification
    private async Task<Boolean> user_id_exists(string username)
    {
        bool? result = await AuthorRepository.AuthorWithUsernameExists(username);

        if (result == true) return true;
        return false;
    }
}