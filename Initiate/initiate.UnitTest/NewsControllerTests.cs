using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initiate.Business;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Initiate.Business.Providers;
using Initiate.Model;
using Initiate.WebAPI.Controllers;

public class NewsControllerTests
{
    private readonly Mock<INewsRepository> mockRepo;
    private readonly Mock<INewsService> mockService;
    private readonly NewsController controller;
    private readonly string username = "user1";
    private readonly string keyword = "test";

    public NewsControllerTests()
    {
        mockRepo = new Mock<INewsRepository>();
        mockService = new Mock<INewsService>();
        controller = new NewsController(mockRepo.Object, mockService.Object);
    }
    
    

    [Fact]
    public async Task GetAllKeywordNews_ReturnsSortedNews()
    {
        // Arrange
        var fakeNewsList = new List<NewsResponse>
        {
            new NewsResponse { PublishedDate = "2021-06-02T00:00:00", ShortTitle = "Short Title2", Title = "Title2", Id = 2 },
            new NewsResponse { PublishedDate = "2021-06-01T00:00:00", ShortTitle = "Short Title1", Title = "Title1", Id = 1 }
        };

        mockRepo.Setup(repo => repo.GetAllKeywordNews(username, keyword)).ReturnsAsync(fakeNewsList);

        // Act
        var result = await controller.GetAllKeywordNews(username, keyword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedNews = Assert.IsType<List<NewsResponse>>(okResult.Value);

        Assert.Equal(2, returnedNews.Count);
        Assert.Equal("2021-06-01T00:00:00", returnedNews[0].PublishedDate); 
        Assert.Equal("2021-06-02T00:00:00", returnedNews[1].PublishedDate);
        Assert.Equal("Short Title1", returnedNews[0].ShortTitle); 
        Assert.Equal("Short Title2", returnedNews[1].ShortTitle);
        Assert.Equal("Title1", returnedNews[0].Title);
        Assert.Equal("Title2", returnedNews[1].Title);
    }
    
    
    [Fact]
    public async Task GetAllLocationNews_ReturnsSortedNews()
    {
        // Arrange
        var fakeNewsList = new List<LocationNewsRespone>
        {
            new LocationNewsRespone { PublishedDate = "2021-06-02T00:00:00", ShortTitle = "Short Title2", Title = "Title2", Keyword = "Keyword1", Id = 2 },
            new LocationNewsRespone { PublishedDate = "2021-06-01T00:00:00", ShortTitle = "Short Title1", Title = "Title1", Keyword = "Keyword2", Id = 1 }
        };
        
        var expectedNewsList = fakeNewsList.OrderBy(n => n.PublishedDate).ToList();
        mockRepo.Setup(repo => repo.GetAllLocationNews(username)).ReturnsAsync(expectedNewsList);

        // Act
        var result = await controller.GetAllLocationNews(username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedNews = Assert.IsType<List<LocationNewsRespone>>(okResult.Value);

        for (int i = 0; i < expectedNewsList.Count; i++)
        {
            Assert.Equal(expectedNewsList[i].PublishedDate, returnedNews[i].PublishedDate);
            Assert.Equal(expectedNewsList[i].ShortTitle, returnedNews[i].ShortTitle);
            Assert.Equal(expectedNewsList[i].Title, returnedNews[i].Title);
            Assert.Equal(expectedNewsList[i].Keyword, returnedNews[i].Keyword);
        }
    }


    [Fact]
    public async Task GetLocationNews_ReturnsNewsDetail()
    {
        // Arrange
        int newsId = 3;
        var fakeNewsDetail = new NewsDetailResponse
        {
            Id = newsId,
            Title = "Detailed Title",
            Content = "Detailed Content",
            PublishedDate = "2021-07-03T00:00:00"
        };

        mockRepo.Setup(repo => repo.GetNews(username, newsId)).ReturnsAsync(fakeNewsDetail);

        // Act
        var result = await controller.GetLocationNews(newsId, username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDetail = Assert.IsType<NewsDetailResponse>(okResult.Value);

        Assert.Equal(fakeNewsDetail.Title, returnedDetail.Title);
        Assert.Equal(fakeNewsDetail.Content, returnedDetail.Content);
        Assert.Equal(fakeNewsDetail.PublishedDate, returnedDetail.PublishedDate);
    }


    [Fact]
    public async Task GetKeywordNews_ReturnsNewsDetail()
    {
        // Arrange
        int newsId = 2;
        var fakeNewsDetail = new NewsDetailResponse
        {
            Id = newsId,
            Title = "Keyword News Title",
            Content = "Keyword News Content",
            PublishedDate = "2021-06-04T00:00:00"
        };

        mockRepo.Setup(repo => repo.GetNews(username, newsId)).ReturnsAsync(fakeNewsDetail);

        // Act
        var result = await controller.GetKeywordNews(newsId, username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDetail = Assert.IsType<NewsDetailResponse>(okResult.Value);

        Assert.Equal(fakeNewsDetail.Title, returnedDetail.Title);
        Assert.Equal(fakeNewsDetail.Content, returnedDetail.Content);
        Assert.Equal(fakeNewsDetail.PublishedDate, returnedDetail.PublishedDate);
    }


    [Fact]
    public async Task GetFirstKeywordNews_ReturnsKeywordNews()
    {
        // Arrange
        var fakeNewsList = new List<NewsResponse>
        {
            new NewsResponse { Id = 1, Title = "First Keyword News", ShortTitle = "FK News", PublishedDate = "2021-08-01T00:00:00" }
        };

        mockService.Setup(service => service.GetKeywordNews(keyword, username)).ReturnsAsync(fakeNewsList);

        // Act
        var result = await controller.GetFirstKeywordNews(username, keyword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedNews = Assert.IsType<List<NewsResponse>>(okResult.Value);

        Assert.Single(returnedNews);
        Assert.Equal(fakeNewsList[0].Title, returnedNews[0].Title);
        Assert.Equal(fakeNewsList[0].ShortTitle, returnedNews[0].ShortTitle);
        Assert.Equal(fakeNewsList[0].PublishedDate, returnedNews[0].PublishedDate);
    }


    [Fact]
    public async Task GetFirstLocationNews_ReturnsLocationNews()
    {
        // Arrange
        var fakeLocationNewsList = new List<LocationNewsRespone>
        {
            new LocationNewsRespone { Id = 4, Title = "First Location News", ShortTitle = "FL News", PublishedDate = "2021-09-01T00:00:00" }
        };

        mockService.Setup(service => service.GetLocationNews(keyword, username)).ReturnsAsync(fakeLocationNewsList);

        // Act
        var result = await controller.GetFirstLocationNews(username, keyword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedNews = Assert.IsType<List<LocationNewsRespone>>(okResult.Value);

        Assert.Single(returnedNews);
        Assert.Equal(fakeLocationNewsList[0].Title, returnedNews[0].Title);
        Assert.Equal(fakeLocationNewsList[0].ShortTitle, returnedNews[0].ShortTitle);
        Assert.Equal(fakeLocationNewsList[0].PublishedDate, returnedNews[0].PublishedDate);
    }

    
}
