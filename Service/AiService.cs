using Application.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class AiService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AiService> _logger;

        public AiService(HttpClient httpClient, ILogger<AiService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetAIResponseAsync(string userMessage)
        {

            // قراءة مفتاح API وعنوان URL من الإعدادات 
            // عدل اسم الموديل من open router حسب ما هو متاح لديك في حسابك
            // يمكنك تجربة موديلات أخرى مثل "arcee-ai/trinity-large-preview:free" أو "meta-llama/llama-3.3-70b-instruct:free"
            // تأكد من أن الموديل الذي تختاره متاح في حسابك على OpenRouter

            var apiKey = _configuration["OpenRouter:ApiKey"];
            var apiUrl = _configuration["OpenRouter:ApiUrl"] ?? "https://openrouter.ai/api/v1/chat/completions";

            try
            {
                // إنشاء JSON بسيط يدويًا - مثل JavaScript تمامًا
                var jsonBody = new
                {
                    //model = "meta-llama/llama-3.3-70b-instruct:free",
                    model = "arcee-ai/trinity-large-preview:free",
                    messages = new[]
                    {
                        new { role = "system", content =
                        " أنت مساعد عربي ذكي. الرجاء الالتزام بالتالي:\r\n    " +
                        "        1. استخدم اللغة العربية الفصحى أو العامية حسب السياق\r\n          " +
                        "  2. تجنب استخدام أي رموز أو أحرف غير معتادة\r\n       " +
                        "     3. نظم الإجابة في فقرات واضحة\r\n        " +
                        "    4. استخدم علامات الترقيم المناسبة" },

                        new { role = "user", content = userMessage }
                                // إذا أردت تحديد اللغة
                    
            // رسالة المستخدم (هذا أساسي)
           
                     }
                };

                _logger.LogInformation($"Sending message to AI: {userMessage}");

                var requestContent = new StringContent(
                    JsonSerializer.Serialize(jsonBody),
                    Encoding.UTF8,
                    "application/json"
                );

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // استخدام await بشكل صحيح
                var response = await _httpClient.PostAsync(apiUrl, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // استخراج المحتوى مباشرة باستخدام JsonDocument
                    using var doc = JsonDocument.Parse(responseContent);
                    var root = doc.RootElement;

                    // استخراج المحتوى بنفس طريقة JavaScript
                    var content = root
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    return content;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"AI Service Error: {response.StatusCode} - {errorContent}");
                    return $"Error: Failed to get response (HTTP {response.StatusCode})";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in AI Service: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}