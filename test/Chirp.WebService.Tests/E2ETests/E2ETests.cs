using Chirp.Tests.Core.Factories;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Chirp.WebService.Tests.E2ETests;

public class E2ETests : IClassFixture<MockWebApplicationFactoryWithAuth>
{
    private readonly MockWebApplicationFactoryWithAuth _fixture;
    
    public E2ETests(MockWebApplicationFactoryWithAuth fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task ClickAuthorNameRedirects()
    {
        //Arrange
        var page = await _fixture.Browser!.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
        
        //XPath for author name
        var authorXPath = "//*[@id='messagelist']/div[2]/div[1]/div[1]/a";
    
        var authorButton = await page.QuerySelectorAsync(authorXPath);
    
        if (authorButton == null) Assert.Fail();
    
        var authorName = await authorButton.InnerTextAsync();
            
        //Act
        await authorButton.ClickAsync();//Simulate click
        
        var firstCheepAuthor = await page.WaitForSelectorAsync("//*[@id='messagelist']/div[1]/div[1]/div/a");
    
        if (firstCheepAuthor == null) Assert.Fail();
        
        var firstCheepAuthorValue = await firstCheepAuthor.InnerTextAsync();
    
        Assert.Contains(authorName, firstCheepAuthorValue);
    }
    
    [Fact]
    public async Task CanDeleteCheep()
    {
        //Arrange
        var page = await _fixture.Browser!.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);

        string cheepMessage = "This is a cheep deleted by the E2E test!";

        var shareButton = await page.QuerySelectorAsync("//*[@id='cheepSubmitButton']");
        
        if (shareButton == null) Assert.Fail();

        //Act
        await page.GetByPlaceholder("Write your own cheep here!").FillAsync(cheepMessage);
        await shareButton.ClickAsync();
        
        //Status: the first cheep should now have been created
        var cheepDeleteButton = await page.QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[2]/div[1]/form[last()]/button[1]");

        if (cheepDeleteButton == null) Assert.Fail();
        
        await cheepDeleteButton.ClickAsync();
        
        //Status: the first cheep should now have been deleted
        var firstCheepLocation = await page.QuerySelectorAsync("//*[@id='messagelist']/div[1]/p");

        if (firstCheepLocation == null) Assert.Fail();
        
        var firstCheepText = await firstCheepLocation.InnerTextAsync();

        Assert.DoesNotContain(cheepMessage, firstCheepText);
    }
    
    [Fact]
    public async Task CanCreateCheep()
    {
        //Arrange
        var page = await _fixture.Browser!.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);
    
        string cheepMessage = "This is a cheep generated by the E2E test!";
        
        var shareButton = await page.QuerySelectorAsync("//*[@id=\"cheepSubmitButton\"]");
    
        if (shareButton == null) Assert.Fail();
    
        //Act
        await page.GetByPlaceholder("Write your own cheep here!").FillAsync(cheepMessage);
        await shareButton.ClickAsync();
    
        //Status: the first cheep should now have been created
    
        var firstCheepLocation = await page.QuerySelectorAsync("//*[@id=\"messagelist\"]/div[1]");
    
        if (firstCheepLocation == null) Assert.Fail();
        
        var firstCheepText = await firstCheepLocation.InnerTextAsync();
    
        Assert.Contains(cheepMessage, firstCheepText);
    }

    [Fact]
    public async Task CommentOnCheepTest()
    {
        //Arrange
        var page = await _fixture.Browser!.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);

        string commentText = "This is a comment generated by the E2E test!";

        var postCommentButton = await page
            .QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[2]/div[2]/form/div/button");
        
        if (postCommentButton == null) Assert.Fail();
        
        //Act
        var firstCheepCommentPlaceholder = await page
            .QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[2]/div[2]/form/div/div/input");
        
        if (firstCheepCommentPlaceholder != null)
        {
            await firstCheepCommentPlaceholder.FillAsync(commentText);
        }
        else
        {
            // Handle the case where the element is not found
            ITestOutputHelper iTestOutputHelper = new TestOutputHelper();
            iTestOutputHelper.WriteLine("Element not found!");
        }
        await postCommentButton.ClickAsync();

        var commentLocation = await page.QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[3]/div/div/div");
        
        if (commentLocation == null) Assert.Fail();

        var commentTextFromLocation = await commentLocation.InnerTextAsync();
        //Assert
        Assert.Contains(commentText, commentTextFromLocation);
    }

    [Fact]
    public async Task DeleteCommentOnCheepTest()
    {
        //Arrange: Create a comment
        var page = await _fixture.Browser!.NewPageAsync();
        await page.GotoAsync(_fixture.BaseUrl);

        string commentText = "This is a comment generated by the E2E test!";
        
        var postCommentButton = await page
            .QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[2]/div[2]/form/div/button");
        
        if (postCommentButton == null) Assert.Fail();
        
        var firstPostCommentPlaceholder = await page
            .QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[2]/div[2]/form/div/div/input");
        
        if (firstPostCommentPlaceholder != null)
        {
            await firstPostCommentPlaceholder.FillAsync(commentText);
        }
        else
        {
            // Handle the case where the element is not found
            ITestOutputHelper iTestOutputHelper = new TestOutputHelper();
            iTestOutputHelper.WriteLine("Element not found!");
        }
        
        await postCommentButton.ClickAsync();
        
        var commentLocation = await page.QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[3]/div/div/div");
        
        if (commentLocation == null) Assert.Fail();
        
        //Act: Delete the cheep
        var commentDeleteButton = await page
            .QuerySelectorAsync("//*[@id=\"messagelist\"]/div[1]/div[3]/div/div/div/div[1]/div[2]/div/form/div/button");
        if (commentDeleteButton == null) Assert.Fail();

        await commentDeleteButton.ClickAsync();

        var commentLocationAfterDeletion = await page
            .QuerySelectorAsync("//*[@id='messagelist']/div[1]/div[3]/div/div/div");
        
        //Assert
        if (commentLocationAfterDeletion == null) //In case there are no other comments
        {
            Assert.True(true);
        } else //In case there are other comments 
        {
            var commentTextAfterDeletion = await commentLocationAfterDeletion.InnerTextAsync();
            Assert.DoesNotContain(commentText, commentTextAfterDeletion);

        }
        

    }
}