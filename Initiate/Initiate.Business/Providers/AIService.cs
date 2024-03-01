using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Initiate.Common;
using Microsoft.Extensions.Configuration;

namespace Initiate.Business.Providers
{
    public class AIService
    {
        static readonly HttpClient client = new HttpClient();


        public async Task<(string ShortTitle, string Content)> GetSummarizedNews(string url)
        {
            string apiKey = Constants.AIApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("AI API key is empty. You must get api key first");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = $"Summarize the news in 100 words or less."+ 
                                                    "Summarize news titles to 4 words or less." +
                                                    "Short Title: 'Here write the Summarized news title'" +
                                                    "Content: here write the Summarized news content" +
                                                    $"This is a news url. {url}"
                    }
                }
            };

            string requestJson = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
                
                return ExtractSummarizedNews(chatResponse.Choices.FirstOrDefault()?.Message.Content);
            }
            else
            {
                return (string.Empty, string.Empty);
            }
        }
        
        
        public (string ShortTitle, string Content) ExtractSummarizedNews(string summarizedText)
        {
            string shortTitlePrefix = "Short Title:";
            string contentPrefix = "Content:";
    
            string shortTitle = string.Empty;
            string content = string.Empty;

            int shortTitleStartIndex = summarizedText.IndexOf(shortTitlePrefix);
            int contentStartIndex = summarizedText.IndexOf(contentPrefix);

            if (shortTitleStartIndex != -1 && contentStartIndex != -1)
            {
                shortTitleStartIndex += shortTitlePrefix.Length;
                contentStartIndex += contentPrefix.Length;

                // Calculate the length for the short title by finding the start of the Content section
                int shortTitleLength = contentStartIndex - shortTitleStartIndex - contentPrefix.Length;

                // Extract Short Title
                if (shortTitleLength > 0)
                {
                    shortTitle = summarizedText.Substring(shortTitleStartIndex, shortTitleLength).Trim();
                }

                // Extract Content
                content = summarizedText.Substring(contentStartIndex).Trim();
            }

            return (ShortTitle: shortTitle, Content: content);
        }

    }
    
    public class ChatCompletionResponse
    {
        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}