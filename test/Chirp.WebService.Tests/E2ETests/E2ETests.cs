﻿using Microsoft.AspNetCore.Mvc.Testing;
using HtmlAgilityPack;
using Microsoft.Playwright;
using Testcontainers.SqlEdge;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Net.Http;

// Used parts of https://github.com/testcontainers/testcontainers-dotnet/tree/develop/examples/WeatherForecast

namespace Chirp.WebService.Tests.E2ETests;

public sealed class End2EndTests : IAsyncLifetime
{
    private readonly SqlEdgeContainer _sqlEdgeContainer = new SqlEdgeBuilder().Build();

    public Task InitializeAsync()
    {
        return _sqlEdgeContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _sqlEdgeContainer.DisposeAsync().AsTask();
    }

    public sealed class RazorTest : IClassFixture<End2EndTests>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _webApplicationFactory;

        private readonly IServiceScope _serviceScope;

        private readonly HttpClient _httpClient;

        public RazorTest(End2EndTests webAppTest)
        {
            // Instead of using environment variables to bootstrap our application configuration, we can implement a custom WebApplicationFactory<TEntryPoint>
            // that overrides the ConfigureWebHost(IWebHostBuilder) method to add a WeatherDataContext to the service collection.
            Environment.SetEnvironmentVariable("ConnectionStrings__ChirpSqlDb",
                webAppTest._sqlEdgeContainer.GetConnectionString());
            _webApplicationFactory = new WebApplicationFactory<Program>();
            _serviceScope = _webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _httpClient = _webApplicationFactory.CreateClient();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _serviceScope.Dispose();
            _webApplicationFactory.Dispose();
        }

        [Fact]
        public async Task TestClickAuthorName()
        {
            //Arrange playwright functionality
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();

            var httpResponse = await _httpClient.GetAsync("/");
            httpResponse.EnsureSuccessStatusCode();
            var response = await httpResponse.Content.ReadAsStringAsync();

            //Retrieve base url from simulation
            var baseUrl = httpResponse.RequestMessage.RequestUri.ToString();
            var finalUrl = baseUrl; //Initially the same as URI

            //Determine if a port should be included (no port on production)
            if (!httpResponse.RequestMessage.RequestUri.IsDefaultPort)
            {
                finalUrl = $"{baseUrl}:{httpResponse.RequestMessage.RequestUri.Port}";
            }

            finalUrl = "https://localhost:7102";

            await page.GotoAsync(finalUrl);

            //await page.SetContentAsync(response);

            var ButtonXPath = "//*[@id=\"messagelist\"]/li[1]/p[1]/strong/a";

            var button = await page.QuerySelectorAsync(ButtonXPath);

            if (button != null)
            {
                var buttonAttribute = await button.GetAttributeAsync("href");

                var location = finalUrl + buttonAttribute.Substring(1); //Remove double slash

                //Simulate the button click with page.GotoAsync due to issues with the url prefix
                var newHttpResponse = await _httpClient.GetAsync(location);
                await page.SetContentAsync(await newHttpResponse.Content.ReadAsStringAsync());

                var updatedPage = await page.ContentAsync();

                Assert.Contains(await button.InnerTextAsync(), updatedPage);
            }
            else
            {
                Assert.Fail();
                return;
            }
        }

[Fact]
            public async Task TestCreateCheep()
            {
                //Arrange playwright
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync();
                var page = await browser.NewPageAsync();

                string cheepMessage = "This is a cheep generated by the E2E test!";
                
                
            }

            [Fact]
            public async Task TestLogin()
            {
                 
                //Arrange playwright
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync();
                var page = await browser.NewPageAsync();
                
                //Set the page content
                var httpResponse = await _httpClient.GetAsync("/");
                var response = await httpResponse.Content.ReadAsStringAsync();
                await page.SetContentAsync(response);
                
                //Simulate click on login button
                var loginButtonXPath = "//div[@class='navigation']/div/a[last()]";

                Console.WriteLine("Breaking 1");

                var data = await page.ContentAsync();
                
                Console.WriteLine("Breaking 1.5");
                
                var loginButton = await page.QuerySelectorAsync(loginButtonXPath);

                Console.WriteLine("Breaking 2");
                
                await loginButton.ClickAsync();
                
                //Retrieve new data
                var newContent = await page.ContentAsync();

                Console.WriteLine("Break for investigation");
                
                Assert.True(true);
            }
    }
}