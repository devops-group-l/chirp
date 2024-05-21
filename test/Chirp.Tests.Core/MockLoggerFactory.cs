using Chirp.WebService.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chirp.Tests.Core;

// Source: https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking

public struct MockLoggerFactories
{
    public ILogger<CheepController> CheepLogger;
    public ILogger<UserController> UserLogger;
    public ILogger<CommentController> CommentLogger;
}

public static class MockLoggerFactory
{
    public static MockLoggerFactories GetMockLoggers()
    {
        var cheepLogger = new Mock<ILogger<CheepController>>
        {
            CallBase = true
        };
        
        var userLogger = new Mock<ILogger<UserController>>
        {
            CallBase = true
        };
        
        var commentLogger = new Mock<ILogger<CommentController>>
        {
            CallBase = true
        };

        return new MockLoggerFactories
        {
            CheepLogger = cheepLogger.Object,
            UserLogger = userLogger.Object,
            CommentLogger = commentLogger.Object
        };
    }
}
