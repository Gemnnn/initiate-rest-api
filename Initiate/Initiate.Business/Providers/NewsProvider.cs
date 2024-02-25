using Initiate.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Business
{
    public class NewsProvider : INewsProvider
    {
        public async Task<NewsDTO> GetNews()
        {
            string baseUrl = "https://gnews.io/api/v4/search?";
            string apiKey = "cb68df24b23a70072eda3fd20b952af2";
            string query = "Ontario AND Conestoga"; // If you want to search using multiple keywords, use AND or OR
            string url = $"{baseUrl}token={apiKey}&q={query}";

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ArticlesResponse>(content);

            }

            return null;
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
        }

    }
}
