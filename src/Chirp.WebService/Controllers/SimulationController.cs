using System.Globalization;
using Chirp.Core.Repositories;
using Chirp.WebService.Controllers.SimulationModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Chirp.Core.Dto;
using Chirp.Infrastructure;

namespace Chirp.WebService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SimulationController : BaseController
{
    private readonly string _latestFileLocation = "latest_processed_sim_action_id.txt";
    private readonly UpdateLatestTracker _updateLatestTracker;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    
    public SimulationController(UpdateLatestTracker updateLatestTracker, IAuthorRepository authorRepository, ICheepRepository cheepRepository, ILikeRepository likeRepository, ICommentRepository commentRepository, ISimulationRepository simulationRepository) : base(authorRepository, cheepRepository, likeRepository, commentRepository, simulationRepository)
    {
        _updateLatestTracker = updateLatestTracker;
        _authorRepository = authorRepository;
        _cheepRepository = cheepRepository;
    }

    [HttpGet("latest")]
    public async Task<ActionResult> HandleLatest()
    {
        // int latestProcessedCommandId;
        //
        // try
        // {
        //     string content = System.IO.File.ReadAllText(_latestFileLocation);
        //     latestProcessedCommandId = int.Parse(content);
        // }
        // catch
        // {
        //     latestProcessedCommandId = -1;//Fallback
        // }

        // return Ok(new {latest = latestProcessedCommandId });

        var latestValue = await SimulationRepository.GetLatest();
        return Ok(new { latest = latestValue });
    }

    [HttpPost("register")]
    public async Task<ActionResult> HandleRegister(HTTPHandleRegisterModel requestModel)
    {
        Infrastructure.Prometheus.endpointRequestsCounter.WithLabels("Register").Inc();
        update_latest(HttpContext);

        string? error = null;
        
        if (string.IsNullOrEmpty(requestModel.username)) error = "You have to enter a username";
        else if (string.IsNullOrEmpty(requestModel.email) || !requestModel.email.Contains("@"))
            error = "You have to enter a valid email address";
        else if (string.IsNullOrEmpty(requestModel.pwd)) error = "You have to enter a password";
        else if (await user_id_exists(requestModel.username)) error = "The username is already taken";

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
        Infrastructure.Prometheus.endpointRequestsCounter.WithLabels("Get messages").Inc();
        update_latest(HttpContext);

        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();

        int noMsgs = 0;
        string? noMsgsString = HttpContext.Request.Query["no"];
        if (int.TryParse(noMsgsString, out int value)) noMsgs = value;

        List<SimulationMessageDto> dtos = await SimulationRepository.GetMessages(noMsgs);

        JArray messages = new JArray();
        foreach(SimulationMessageDto dto in dtos)
        {
            JObject newMsg = new JObject
            {
                { "content", dto.text },
                { "pub_date", dto.pub_date },
                { "user", dto.username }
            };
            messages.Add(newMsg);
        }

        return Ok(messages.ToString());
    }
    
    [HttpGet("msgs/{username}")]
    public async Task<ActionResult> HandleMsgsUsernameGet(string username)
    {
        Infrastructure.Prometheus.endpointRequestsCounter.WithLabels("Get messages from user").Inc();
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();
        
        int noMsgs = 0;
        string? noMsgsString = HttpContext.Request.Query["no"];
        if (int.TryParse(noMsgsString, out int value)) noMsgs = value;

        if (! await user_id_exists(username)) return NotFound();

        List<SimulationMessageDto> dtos = await SimulationRepository.GetSpecificMessages(username, noMsgs);
        
        JArray messages = new JArray();
        foreach(SimulationMessageDto dto in dtos)
        {
            JObject newMsg = new JObject
            {
                { "content", dto.text },
                { "pub_date", dto.pub_date },
                { "user", dto.username }
            };
            messages.Add(newMsg);
        }

        return Ok(messages.ToString());
    }

    [HttpPost("msgs/{username}")]
    public async Task<ActionResult> HandleMsgsUsernamePost(HTTPPostMessageModel requestModel, string username)
    {
        Infrastructure.Prometheus.endpointRequestsCounter.WithLabels("Post message").Inc();
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();

        await SimulationRepository.AddMessage(new SimulationMessageDto()
        { 
            username = username,
            text = requestModel.content,
            pub_date = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture)
        });

        return NoContent();
    }
    
    [HttpGet("fllws/{username}")]
    public async Task<ActionResult> HandleFllwsUsernameGet(string username)
    {
        Infrastructure.Prometheus.endpointRequestsCounter.WithLabels("Get followers").Inc();
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();

        bool userExists = await user_id_exists(username);
        if (!userExists) return NotFound();

        int noFollowers = 0;
        string? noFollowersString = HttpContext.Request.Query["no"];
        if (int.TryParse(noFollowersString, out int value)) noFollowers = value;

        List<string> follows = await SimulationRepository.GetFollowsForUser(username, noFollowers);

        JObject returnable = new JObject
        {
            { "follows", JArray.FromObject(follows) }
        };

        return Ok(returnable);
    }

    [HttpPost("fllws/{username}")]
    public async Task<ActionResult> HandleFllwsUsernamePost(HTTPHandleFollowModel requestModel, string username)
    {
        
        update_latest(HttpContext);
        
        bool illegalRequest = not_req_from_simulator(HttpContext);
        if (illegalRequest) return ForbidAccess();

        if (! await user_id_exists(username)) return NotFound();
        
        //Determine if follow or unfollow request
        if (requestModel.follow != null)
        {
            Infrastructure.Prometheus.endpointRequestsCounter.WithLabels("Follow user").Inc();
            if (! await user_id_exists(requestModel.follow)) return NotFound();

            await SimulationRepository.AddFollower(username, requestModel.follow);
            return NoContent();
        }

        if (requestModel.unfollow != null)
        {
            Infrastructure.Prometheus.endpointRequestsCounter.WithLabels("Unfollow user").Inc();
            if (! await user_id_exists(requestModel.unfollow)) return NotFound();

            SimulationRepository.RemoveFollower(username, requestModel.unfollow);
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
        catch
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
        
        if (!string.IsNullOrEmpty(parsedCommandIdString))
        {
            if (int.TryParse(parsedCommandIdString, out int intValue)) parsedCommandId = intValue;
            SimulationRepository.UpdateLatest(parsedCommandId);
        }
    }
    
    private async Task<bool> user_id_exists(string username)
    {
        //return SimulationRepository.CheckIfUserExists(username);
        var exists = await _authorRepository.AuthorWithUsernameExists(username.Trim());
        return exists == true;
    }
}