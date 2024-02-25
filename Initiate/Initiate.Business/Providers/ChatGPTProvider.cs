using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Business.Providers
{
    public class ChatGPTProvider : IChatGPTProvider
    {
        static readonly HttpClient client = new HttpClient();

        public async Task GetSummerizeedNews()
        {

            //var apiKey = "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";
            string apiKey = "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";
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo", // Specify the model, you might want to check the latest models in OpenAI documentation
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = "Tell me a joke." }
                }
            };

            string requestJson = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from OpenAI:");
                Console.WriteLine(responseContent);
            }
            else
            {
                Console.WriteLine($"Failed to get response. Status code: {response.StatusCode}");
            }
            return;
        }
    }
}
