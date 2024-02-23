using Chirp.Core.Repositories;
using Chirp.WebService.Controllers.SimulationModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using Chirp.Core.Dto;
using Chirp.Infrastructure.Repositories;

namespace Chirp.WebService.Controllers;
[Route("api/[controller]")]//TODO: CHANGE
[ApiController]
public class SimulationController : BaseController
{
    private readonly string _latestFileLocation = "./SimulationModels/latest_processed_sim_action_id.txt";
    
    public SimulationController(IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, ISimulationRepository simulationRepository) : base(authorRepository, cheepRepository, likeRepository, commentRepository, simulationRepository)
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
        
        if (string.IsNullOrEmpty(requestModel.username)) error = "You have to enter a username";
        else if (string.IsNullOrEmpty(requestModel.email) || !requestModel.email.Contains("@"))
            error = "You have to enter a valid email address";
        else if (string.IsNullOrEmpty(requestModel.pwd)) error = "You have to enter a password";
        else if (user_id_exists(requestModel.username)) error = "The username is already taken";

        if (error != null)
        {
            JObject returnModel = new JObject()
            {
                { "status", 400 },
                { "error_msg", error }
            };

            return BadRequest(returnModel.ToString());
        }
        
        //Create user
        await SimulationRepository.AddUser(new SimulationUserDto()
        {
            Email = requestModel.email,
            Username = requestModel.username,
            PwdHashed = requestModel.pwd,
        });//TODO: HASH PWD?

        return NoContent();//HTTP 204
    }

    [HttpGet("msgs")]
    public async Task<ActionResult> HandleMsgs()
    {
        update_latest(HttpContext);

        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();

        int noMsgs = 0;
        string noMsgsString = HttpContext.Request.Query["no"];
        if (int.TryParse(noMsgsString, out int value)) noMsgs = value;

        List<SimulationMessageDto> dtos = await SimulationRepository.GetMessages(noMsgs);

        JArray messages = new JArray();
        foreach(SimulationMessageDto dto in dtos)
        {
            JObject newMsg = new JObject();
            newMsg.Add("Content", dto.text);
            newMsg.Add("pub_date", dto.pub_date);
            newMsg.Add("user", dto.username);
            messages.Add(newMsg);
        }

        return Ok(messages.ToString());
    }
    
    [HttpGet("msgs/{username}")]
    public async Task<ActionResult> HandleMsgsUsernameGet(string username)
    {
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();
        
        int noMsgs = 0;
        string noMsgsString = HttpContext.Request.Query["no"];
        if (int.TryParse(noMsgsString, out int value)) noMsgs = value;

        if (!user_id_exists(username)) return NotFound();

        List<SimulationMessageDto> dtos = await SimulationRepository.GetSpecificMessages(username, noMsgs);
        
        JArray messages = new JArray();
        foreach(SimulationMessageDto dto in dtos)
        {
            JObject newMsg = new JObject();
            newMsg.Add("Content", dto.text);
            newMsg.Add("pub_date", dto.pub_date);
            newMsg.Add("user", dto.username);
            messages.Add(newMsg);
        }

        return Ok(messages);
    }

    [HttpPost("msgs/{username}")]
    public async Task<ActionResult> HandleMsgsUsernamePost(HTTPPostMessageModel requestModel)
    {
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();

        SimulationRepository.AddMessage(new SimulationMessageDto()
        { 
            username = requestModel.username,
            text = requestModel.content,
            pub_date = DateTime.UtcNow.ToString()
        });

        return NoContent();
    }
    
    //TODO: Implement remaining GET/POST for fllws/username
    [HttpGet("fllws/{username}")]
    public async Task<ActionResult> HandleFllwsUsernameGet(string username)
    {
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();

        bool userExists = user_id_exists(username);
        if (!userExists) return NotFound();

        int noFollowers = 0;
        string noFollowersString = HttpContext.Request.Query["no"];
        if (int.TryParse(noFollowersString, out int value)) noFollowers = value;

        List<string> follows = SimulationRepository.GetFollowsForUser(username, noFollowers);

        JObject returnable = new JObject
        {
            { "follows", JArray.FromObject(follows) }
        };

        return Ok(returnable);
    }

    [HttpPost("fllws/{username}")]
    public async Task<ActionResult> HandleFllwsUsernamePost(HTTPHandleFollowModel requestModel)
    {
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();
        
        bool userExists = user_id_exists(requestModel.username);
        if (!userExists) return NotFound();
        
        //Determine if follow or unfollow request
        if (requestModel.follow != null)
        {
            bool followExist = user_id_exists(requestModel.follow);
            if (!followExist) return NotFound();

            SimulationRepository.AddFollower(requestModel.username, requestModel.follow);
            return NoContent();
        }

        if (requestModel.unfollow != null)
        {
            bool followExist = user_id_exists(requestModel.unfollow);
            if (!followExist) return NotFound();

            SimulationRepository.RemoveFollower(requestModel.username, requestModel.unfollow);
            return NoContent();
        }

        return BadRequest();
    }
    
    //Helper functions
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

    private ActionResult ForbidAccess()
    {
        string error = "You are not authorized to use this resource!";
        JObject returnObject = new JObject
        {
            { "status", 403 },
            { "error_msg", error }
        };

        return StatusCode(403, returnObject.ToString());
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
    
    private Boolean user_id_exists(string username)
    {
        return SimulationRepository.CheckIfUserExists(username);
    }
}