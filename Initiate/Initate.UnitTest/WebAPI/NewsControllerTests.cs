using AutoMapper;
using Initiate.Model;

public class NewsRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly NewsRepository _newsRepository;

    public NewsRepositoryTests()
    {
        // Set up DbContext options to use an in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryNewsDb")
            .Options;

        _dbContext = new ApplicationDbContext(options);

        // Initialize mapper
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile()); 
        });
        IMapper mapper = mappingConfig.CreateMapper();

        // Create the repository instance
        _newsRepository = new NewsRepository(_dbContext, mapper);

        SeedDatabase();
    }
    private void SeedDatabase()
    {
        // Clear existing data
        _dbContext.News.RemoveRange(_dbContext.News);
        _dbContext.SaveChanges();

        // Seed new data
        var newsItems = new List<News>
        {
            new News { Title = "Test News 1", Content = "Content 1", Source = "Test Source 1" },
            new News { Title = "Test News 2", Content = "Content 2", Source = "Test Source 2" }
        };

        _dbContext.News.AddRange(newsItems);
        _dbContext.SaveChanges();
    }


    [Fact]
    public async Task GetAllNews_ReturnsAllNews()
    {
        // Act
        var result = await _newsRepository.GetAllNews();

        // Assert
        Assert.NotNull(result);
        var newsList = result.ToList();
        Assert.Equal(2, newsList.Count);
    }

    [Fact]
    public async Task GetNews_ReturnsNews_WhenNewsExists()
    {
        // Act
        var result = await _newsRepository.GetNews(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test News 1", result.Title);
    }

    //[Fact]
    //public async Task CreateNews_AddsNewsToDatabase()
    //{
    //    // Arrange
    //    var newNewsDTO = new NewsDTO { Title = "New Test News", Content = "New Test Content", Source = "Test Source" };

    //    // Act
    //    var createdNewsDTO = await _newsRepository.CreateNews(newNewsDTO);

    //    // Assert
    //    Assert.NotNull(createdNewsDTO);
    //    var newsInDb = await _dbContext.News.FindAsync(createdNewsDTO.Id);
    //    Assert.NotNull(newsInDb);
    //    Assert.Equal(newNewsDTO.Title, newsInDb.Title);
    //}

    //[Fact]
    //public async Task UpdateNews_UpdatesExistingNews()
    //{
    //    // Arrange
    //    var existingNews = await _dbContext.News.FirstAsync();
    //    var updatedNewsDTO = new NewsDTO { Id = existingNews.Id, Title = "Updated Title", Content = "Updated Content", Source = existingNews.Source };

    //    // Act
    //    var updatedDTO = await _newsRepository.UpdateNews(existingNews.Id, updatedNewsDTO);

    //    // Assert
    //    Assert.NotNull(updatedDTO);
    //    var newsInDb = await _dbContext.News.FindAsync(existingNews.Id);
    //    Assert.NotNull(newsInDb);
    //    Assert.Equal(updatedNewsDTO.Title, newsInDb.Title);
    //}

    //[Fact]
    //public async Task DeleteNews_RemovesNewsFromDatabase()
    //{
    //    // Arrange
    //    var existingNews = await _dbContext.News.FirstAsync();

    //    // Act
    //    var result = await _newsRepository.DeleteNews(existingNews.Id);

    //    // Assert
    //    Assert.Equal(1, result);
    //    var newsInDb = await _dbContext.News.FindAsync(existingNews.Id);
    //    Assert.Null(newsInDb);
    //}

    // Dispose the in-memory context after each test
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
