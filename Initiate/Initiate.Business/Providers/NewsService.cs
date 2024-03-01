﻿using Initiate.Business.Providers;
using Initiate.Model;
using Newtonsoft.Json;
using Initiate.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Initiate.Business
{
    public class NewsService : INewsService
    {
        private Dictionary<string, UserTimer> m_userTimers;
        private readonly IServiceScopeFactory m_scopeFactory;

        public NewsService(IServiceScopeFactory scopeFactory)
        {
            m_scopeFactory = scopeFactory;
            m_userTimers = new Dictionary<string, UserTimer>();
        }

        public async Task Initialize()
        {
            using (var scope = m_scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var users = await dbContext.Users.Include(x => x.Preference).ToListAsync();
                foreach (var user in users)
                {
                    TimeSpan timeSpan = TimeSpan.Parse(user.Preference.NewsGenerationTime);
                    m_userTimers[user.UserName] = new UserTimer(timeSpan);
                    SetDailyEventForUser(user.UserName);
                }
            }
        }

        public async Task UpdateTimer(string username, string date)
        {
            TimeSpan timeSpan = TimeSpan.Parse(date);
            if (m_userTimers.ContainsKey(username))
                m_userTimers.Remove(username);

            m_userTimers.Add(username, new UserTimer(timeSpan));
            SetDailyEventForUser(username);
        }

        private void SetDailyEventForUser(string userName)
        {
            if (!m_userTimers.ContainsKey(userName))
                return;

            var userTimer = m_userTimers[userName];
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, userTimer.GenerateTime.Hours,
                userTimer.GenerateTime.Minutes, 0);

            if (now > scheduledTime)
                scheduledTime = scheduledTime.AddDays(1);

            double initialDelay = (scheduledTime - now).TotalMilliseconds;

            userTimer.Timer = new Timer(e => TimerEvent(userName), null, (int)initialDelay, Timeout.Infinite);
            m_userTimers[userName] = userTimer;
        }

        private void TimerEvent(string username)
        {
            SetDailyEventForUser(username);
        }

        private async Task<User> GetUser(string username)
        {
            User user = null;
            try
            {
                using (var scope = m_scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    user = await dbContext.Users.Include(x=>x.Keywords).FirstAsync(x => x.UserName == username);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed to get user. {e.Message}";
                Console.WriteLine(message);
                throw;
            }
            
            return user;
        }
        
        private async Task<string> GetLocation(string username)
        {
            User user = null;
            try
            {
                using (var scope = m_scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    user = await dbContext.Users.Include(x=>x.Preference)
                        .FirstAsync(x => x.UserName == username);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed to get user. {e.Message}";
                Console.WriteLine(message);
                throw;
            }
            
            return user.Preference.Province;
        }
        
        private async Task<string> GetKeywords(string username)
        {
            try
            {
                using (var scope = m_scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    var user = await dbContext.Users.Include(x=>x.Keywords).FirstAsync(x=>x.UserName == username);
                    var keywords = user.Keywords.Select(x=>x.Word);
                    if (keywords == null)
                        return string.Empty;
                    
                    return string.Join(" OR ", keywords); ;
                }
            }
            catch (Exception e)
            {
                var message = $"Failed to get keywords. {e.Message}";
                Console.WriteLine(message);
                throw;
            }
        }

        public async Task<IEnumerable<NewsResponse>> GetKeywordNews(string keyword, string username)
        {
            var yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var user = await GetUser(username);

            if (user == null)
                throw new Exception($"No User Found. User: {username}");
            
            if(user.Keywords.Any(x=>x.Word.ToLower() == keyword.ToLower()) == false)
                throw new Exception($"No Keyword Found: {keyword}");

            string baseUrl = "https://gnews.io/api/v4/search?";
            string apiKey = "cb68df24b23a70072eda3fd20b952af2";
            string url = $"{baseUrl}token={apiKey}&q={keyword}&from={yesterday}&to={today}&sortby=relevance";

            List<News> newsList = new List<News>();
            HashSet<string> titles = new HashSet<string>();

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get news");

            string articles = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ArticlesResponse>(articles);

            foreach (var article in result.Articles)
            {
                try
                {
                    if (!titles.Contains(article.Title) && !newsList.Any(n => n.Source == article.Url))
                    {
                        titles.Add(article.Title);

                        AIService aiService = new AIService();
                        (var shortTitle, var summarizedNews) =
                            await aiService.GetSummarizedNews(article.Content);

                        newsList.Add(new News
                        {
                            Title = article.Title,
                            ShortTitle = shortTitle,
                            Content = summarizedNews,
                            Source = article.Url,
                            PublishedDate = DateTime.Now,
                            Desciprtion = article.Description,
                            Author = "Unknown",
                            Keyword = keyword
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            var storedNewsList = await StoreNewsInDB(username, newsList);

            var newsResponseList = storedNewsList.Select(news => new NewsResponse
            {
                Id = news.NewsId,
                Title = news.Title,
                ShortTitle = news.ShortTitle,
                PublishedDate = news.PublishedDate.ToString("yyyy-MM-dd")
            });

            return newsResponseList;
        }

        private async Task<List<News>> StoreNewsInDB(string username, IEnumerable<News> newsList)
        {
            List<News> storedNewsList = new List<News>();
            try
            {
                using (var scope = m_scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = await dbContext.Users.Include(x => x.News).FirstOrDefaultAsync(x => x.UserName == username);

                    if (user != null)
                    {
                        foreach (var news in newsList)
                        {
                            var addedNews = dbContext.News.Add(news);
                            user.News.Add(addedNews.Entity); 
                            storedNewsList.Add(addedNews.Entity); 
                        }

                        await dbContext.SaveChangesAsync(); 
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to store news in database. {e.Message}");
            }
    
            return storedNewsList;
        }


        public async Task<IEnumerable<NewsResponse>> GetLocationNews(string location, string username)
        {
            var yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var user = await GetUser(username);
            var province = await GetLocation(username);
            var keywords = await GetKeywords(username);
            var query = province + "AND" + keywords;

            if (user == null)
                throw new Exception($"No User Found. User: {username}");
            
            if (string.IsNullOrWhiteSpace(province) == true)
                throw new Exception($"No province Found. Province: {province}");

            
            string baseUrl = "https://gnews.io/api/v4/search?";
            string apiKey = "cb68df24b23a70072eda3fd20b952af2";
            string url = $"{baseUrl}token={apiKey}&q={query}&from={yesterday}&to={today}&sortby=relevance";
            
            List<News> newsList = new List<News>();
            HashSet<string> titles = new HashSet<string>();
            
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get news");
            
            string articles = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ArticlesResponse>(articles);
            
            foreach (var article in result.Articles)
            {
                try
                {
                    if (!titles.Contains(article.Title) && !newsList.Any(n => n.Source == article.Url))
                    {
                        titles.Add(article.Title);
            
                        AIService aiService = new AIService();
                        (var shortTitle, var summarizedNews) =
                            await aiService.GetSummarizedNews(article.Content);
            
                        newsList.Add(new News
                        {
                            Title = article.Title,
                            ShortTitle = shortTitle,
                            Content = summarizedNews,
                            Source = article.Url,
                            PublishedDate = DateTime.Now,
                            Desciprtion = article.Description,
                            Author = "Unknown",
                            IsLocation = true
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            var storedNewsList = await StoreNewsInDB(username,newsList);
            
            var newsResponseList = storedNewsList.Select(news => new NewsResponse
            {
                Id = news.NewsId,
                Title = news.Title,
                ShortTitle = news.ShortTitle,
                PublishedDate = news.PublishedDate.ToString("yyyy-MM-dd")
            });
            
            return newsResponseList;
        }

        public class UserTimer
        {
            public UserTimer(TimeSpan generateTime)
            {
                GenerateTime = generateTime;
            }

            public Timer Timer { get; set; }
            public TimeSpan GenerateTime { get; set; }
        }

        public class ArticlesResponse
        {
            [JsonProperty("totalArticles")] 
            public int TotalArticles { get; set; }

            [JsonProperty("articles")] 
            public List<Article> Articles { get; set; }
        }

        public class Article
        {
            [JsonProperty("title")] 
            public string Title { get; set; }
            [JsonProperty("description")] 
            public string Description { get; set; }
            [JsonProperty("content")] 
            public string Content { get; set; }
            [JsonProperty("publishedAt")] 
            public string PublishedDate { get; set; }
            [JsonProperty("image")] 
            public string Image { get; set; }
            [JsonProperty("url")] 
            public string Url { get; set; }
        }
    }
}