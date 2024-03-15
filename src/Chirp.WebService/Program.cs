using Chirp.Core.Repositories;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Repositories;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.Data.SqlClient;
using Prometheus;

namespace Chirp.WebService;

// Used https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/data/ef-rp/intro/samples/cu50 as inspiration

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // host.Services.GetService<ChirpDbContext>()?.Database.EnsureCreated();
        
        host.Run();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
            })
            .ConfigureHostConfiguration(builder =>
            {
                builder.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(context.Configuration, services);
            });

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddHttpClient();
        services
            .AddRazorPages(_ => { })
            .AddMvcOptions(_ => { });
        
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<ICheepRepository, CheepRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ISimulationRepository, SimulationRepository>();
        services.AddSingleton(configuration);
        services.AddSingleton(new UpdateLatestTracker());
        services.UseHttpClientMetrics();

        services.AddSession(options =>
        {
            options.Cookie.Name = ".Chirp.Session";
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.IsEssential = true;
        });
        
        var sqlConnectionString = new SqlConnectionStringBuilder(configuration.GetConnectionString("ChirpSqlDb"))
            {
                Pooling = true
            };
        Console.WriteLine($"Connection String: {sqlConnectionString}");
        string? password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (string.IsNullOrEmpty(sqlConnectionString.Password) && password != null)
        {
            // Add local password
            sqlConnectionString.Password = password;
            Console.WriteLine($"Password from environment variable: {password}");
        } 
        
        services.AddDbContext<ChirpDbContext>(options =>
            options.UseSqlServer(sqlConnectionString.ConnectionString));
    }
}

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using var server = new KestrelMetricServer(port: 8080);
        server.Start();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        // app.Use(async (context, next) =>
        // {
        //     // Log request time per path
        //     var path = (context.Request.Path.Value ?? "").Replace("/", "_").Replace(" ", "_");
        //     var isSimulationEndpoint = path.Contains("/Simulation/");
        //     if (Infrastructure.Prometheus.ReqDurations.ContainsKey(path))
        //     {
        //         using (Infrastructure.Prometheus.ReqDurations[path].NewTimer())
        //         { 
        //             await next.Invoke();
        //
        //         }
        //     }
        //     else
        //     {
        //         var histogram = Metrics.CreateHistogram($"minitwit_request_duration_miliseconds_{path}", $"Request duration for endpoint {path}");
        //         Infrastructure.Prometheus.ReqDurations.Add(path, histogram);
        //         using (histogram.NewTimer())
        //         { 
        //             await next.Invoke();
        //
        //         }
        //     }
        //     
        //     // Log request count
        //     if (isSimulationEndpoint)
        //     {
        //         Infrastructure.Prometheus.ResponseCounterSim.Inc();
        //     }
        //     else
        //     {
        //         Infrastructure.Prometheus.ResponseCounter.Inc();
        //     }
        // });

        

        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpMetrics();

        // Use the custom middleware
        app.Use(async (context, next) =>
        {
            // var authorRepository = context.RequestServices.GetRequiredService<IAuthorRepository>();

            if (context.Session.TryGetValue("UserId", out var userIdByte))
            {
                var userId = new Guid(userIdByte);

                // Store the userId in a way that it can be accessed during the request
                // context.Items["user_id"] = userId;

                // // Optionally, query the database using userId
                // var user = await authorRepository.GetAuthorById(userId);
                //
                // if (user == null)
                // {
                //     // User not found in the database
                //     // Log an error message
                //     Console.WriteLine($"User with ID {userId} not found in the database.");
                //
                //     // Redirect to the homepage with an error message
                //     context.Response.Redirect("/?error=UserNotFound");
                //     return;
                // }
                // Store the user information for later use 
                context.Items["userId"] = userId;
            }

            await next.Invoke();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapMetrics();
        });
    }
}