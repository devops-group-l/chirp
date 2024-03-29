using System.ComponentModel.DataAnnotations;
using Bogus;
using Chirp.Infrastructure.Models;
using Chirp.Tests.Core;

namespace Chirp.Infrastructure.Tests.Models;

public class ModelsUnitTests
{
    [Fact]
    public void TestAuthorFields()
    {
        string name = new Faker().Name.FullName();
        string username = new Faker().Internet.UserName(name);
        string avatarUrl = new Faker().Internet.Avatar();
        string password = new Faker().Internet.Password();

        Author author = new Author
        {
            Username = username,
            AvatarUrl = avatarUrl,
            Password = password
        };
        
        Assert.Equal(username, author.Username);
        Assert.Equal(avatarUrl, author.AvatarUrl);
        Assert.Equal(Guid.Empty, author.AuthorId);
        Assert.Equal(password, author.Password);
        Assert.Empty(author.Cheeps);
    }

    [Fact]
    public void TestLikeFields()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        var cheepId = Guid.NewGuid();
        var cheep = new Cheep
        {
            CheepId = cheepId,
            Author = author,
            Text = new Faker().Random.Words()
        };
        Like like = new Like
        {
            LikedByAuthor = author,
            Cheep = cheep
        };
        
        Assert.Equal(author.AuthorId.ToString(), like.LikedByAuthor.AuthorId.ToString());
        Assert.Equal(cheepId.ToString(), like.Cheep.CheepId.ToString());
    }

    [Fact]
    public void TestCheepFields()
    {
        Author author = DataGenerator.GenerateAuthorFaker().Generate();

        string text = new Faker().Random.Words();

        Cheep cheep = new Cheep
        {
            Author = author,
            Text = text
        };

        Assert.Equal(text, cheep.Text);
        Assert.Equal(author.AuthorId, cheep.Author.AuthorId);
    }

    [Fact]
    public void TestCommentFields()
    {
        Author author = DataGenerator.GenerateAuthorFaker().Generate();
        String text = new Faker().Random.Words();

        Cheep cheep = new Cheep
        {
            Author = author,
            Text = text
        };

        String commentText = new Faker().Random.Words();
        Comment comment = new Comment
        {
            Cheep = cheep,
            CommentAuthor = author,
            CommentId = Guid.NewGuid(),
            Text = commentText
        };
        
        Assert.Equal(commentText, comment.Text);
        Assert.Equal(author.AuthorId, comment.CommentAuthor.AuthorId);
        Assert.Equal(cheep.CheepId, comment.Cheep.CheepId);
    }

    [Fact]
    public void TestCheepModelCorrectTimestamp()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        var cheep = new Cheep
        {
            Author = author,
            Text = new Faker().Random.Words()
        };
        var timeNowMinusOneSecond = DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1)).ToFileTimeUtc();
        var cheepTime = cheep.Timestamp.ToFileTimeUtc();

        Assert.True(cheepTime > timeNowMinusOneSecond);
    }
    

    [Fact]
    public void ExceptionTestCheepLengthMax()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        Cheep cheep = new Cheep
        {
            Author = author,
            Text = new string('a', 161)
        };

        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(cheep, new ValidationContext(cheep), true));
        Assert.Contains("Cheeps must contain less than 160 characters", exception.Message);
    }

    [Fact]
    public void ExceptionTestCheepLengthMin()
    {
        var author = DataGenerator.GenerateAuthorFaker().Generate();
        Cheep cheep = new Cheep
        {
            Author = author,
            Text = ""
        };

        var exception = Assert.Throws<System.ComponentModel.DataAnnotations.ValidationException>
            (() => Validator.ValidateObject(cheep, new ValidationContext(cheep), true));
        Assert.Contains("The Text field is required", exception.Message);
    }

}