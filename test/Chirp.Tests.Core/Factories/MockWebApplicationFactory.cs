using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chirp.Tests.Core.Factories;

public class MockWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        Environment.SetEnvironmentVariable(
            "DB_STRING",
            "Host=random-host.com;Port=1234;Database=defaultdb;Username=admin;Password=password"
        );

        builder.ConfigureTestServices(servicesConfiguration =>
        {
            var mockRepositories = MockRepositoryFactory.GetMockCheepRepositories();

            servicesConfiguration.AddScoped<ICheepRepository>(_ =>
                mockRepositories.CheepRepository
            );
            servicesConfiguration.AddScoped<IAuthorRepository>(_ =>
                mockRepositories.AuthorRepository
            );

            servicesConfiguration.AddScoped<ILikeRepository>(_ => mockRepositories.LikeRepository);
            servicesConfiguration.AddScoped<ICommentRepository>(_ =>
                mockRepositories.CommentRepository
            );
            servicesConfiguration.AddScoped<ISimulationRepository>(_ =>
                mockRepositories.SimulationRepository
            );

            var descriptor = servicesConfiguration.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ChirpDbContext>)
            );
            if (descriptor != null)
                servicesConfiguration.Remove(descriptor);
        });

        builder.UseEnvironment("Development");
    }
}
