using Newtonsoft.Json;
using System.Text;

namespace Real_Time_Chat_Backend.Services
{
    public class FreeAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FreeAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            try
            {
                // Try to get response from free AI service (no auth required)
                var aiResponse = await GetFreeAIResponse(userMessage);
                if (!string.IsNullOrEmpty(aiResponse))
                {
                    return aiResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Service Error: {ex.Message}");
            }

            // Fallback to smart keyword-based responses
            return GetSmartFallbackResponse(userMessage);
        }

        private async Task<string> GetFreeAIResponse(string userMessage)
        {
            try
            {
                // Using a completely free AI service that doesn't require authentication
                var requestData = new
                {
                    prompt = $"User: {userMessage}\nAssistant:",
                    max_tokens = 100,
                    temperature = 0.7
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Using a free AI endpoint (you can change this URL to other free services)
                var response = await _httpClient.PostAsync(
                    "https://api.free-ai-chat.com/generate", // This is a placeholder URL
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Parse response based on the service format
                    return ParseAIResponse(responseContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Free AI API Error: {ex.Message}");
            }

            return string.Empty;
        }

        private string ParseAIResponse(string responseContent)
        {
            try
            {
                // Try to parse different response formats
                if (responseContent.Contains("text") || responseContent.Contains("response"))
                {
                    var response = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return response?.text?.ToString() ?? response?.response?.ToString() ?? "";
                }
            }
            catch
            {
                // If parsing fails, return empty string to use fallback
            }
            return string.Empty;
        }

        private string GetSmartFallbackResponse(string userMessage)
        {
            var lowerMessage = userMessage.ToLower();
            
            // Enhanced keyword-based responses with more variety
            if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi") || lowerMessage.Contains("hey"))
            {
                var greetings = new[]
                {
                    "Hello! 👋 How can I help you today? I'm here to assist with any questions you might have!",
                    "Hi there! 😊 Great to see you! What can I help you with today?",
                    "Hey! ✨ Welcome! I'm your AI assistant, ready to help!"
                };
                return greetings[new Random().Next(greetings.Length)];
            }
            else if (lowerMessage.Contains("how are you"))
            {
                var responses = new[]
                {
                    "I'm doing great, thank you for asking! 😊 How about you? I'm always ready to help!",
                    "I'm wonderful! 🌟 Thanks for checking in. How are you doing today?",
                    "I'm excellent! 💫 Ready to assist you with anything you need!"
                };
                return responses[new Random().Next(responses.Length)];
            }
            else if (lowerMessage.Contains("weather"))
            {
                return "I'd love to help with weather info, but I don't have real-time access to weather data. You might want to check a weather app or website! 🌤️";
            }
            else if (lowerMessage.Contains("time"))
            {
                return $"The current time is {DateTime.Now.ToString("HH:mm:ss")} ⏰";
            }
            else if (lowerMessage.Contains("date"))
            {
                return $"Today's date is {DateTime.Now.ToString("dddd, MMMM dd, yyyy")} 📅";
            }
            else if (lowerMessage.Contains("help"))
            {
                return "I'm here to help! 🤖 I can answer questions, provide information, tell jokes, or just chat. What would you like to know?";
            }
            else if (lowerMessage.Contains("bye") || lowerMessage.Contains("goodbye"))
            {
                var goodbyes = new[]
                {
                    "Goodbye! 👋 Have a wonderful day! Feel free to come back anytime!",
                    "See you later! ✨ Take care and have a great day!",
                    "Bye! 🌟 It was nice chatting with you! Come back soon!"
                };
                return goodbyes[new Random().Next(goodbyes.Length)];
            }
            else if (lowerMessage.Contains("name"))
            {
                return "My name is AI Assistant! 🤖 Nice to meet you! I'm here to help with any questions or just chat!";
            }
            else if (lowerMessage.Contains("joke"))
            {
                var jokes = new[]
                {
                    "Why don't scientists trust atoms? Because they make up everything! 😄",
                    "Why did the scarecrow win an award? Because he was outstanding in his field! 🌾",
                    "What do you call a fake noodle? An impasta! 🍝",
                    "Why don't eggs tell jokes? They'd crack each other up! 🥚",
                    "What do you call a bear with no teeth? A gummy bear! 🐻",
                    "Why did the math book look so sad? Because it had too many problems! 📚",
                    "What do you call a fish wearing a bowtie? So-fish-ticated! 🐠",
                    "Why don't skeletons fight each other? They don't have the guts! 💀"
                };
                return jokes[new Random().Next(jokes.Length)];
            }
            else if (lowerMessage.Contains("thank"))
            {
                var thanks = new[]
                {
                    "You're very welcome! 😊 I'm here to help anytime!",
                    "My pleasure! 🌟 Happy to assist you!",
                    "Anytime! ✨ That's what I'm here for!"
                };
                return thanks[new Random().Next(thanks.Length)];
            }
            else if (lowerMessage.Contains("love") || lowerMessage.Contains("like"))
            {
                return "That's wonderful! 💕 I'm glad you feel that way!";
            }
            else if (lowerMessage.Contains("music") || lowerMessage.Contains("song"))
            {
                return "Music is amazing! 🎵 What kind of music do you enjoy? I'd love to hear about your favorite artists!";
            }
            else if (lowerMessage.Contains("food") || lowerMessage.Contains("eat"))
            {
                return "Food is one of life's great pleasures! 🍕 What's your favorite cuisine? I'm always curious about different tastes!";
            }
            else if (lowerMessage.Contains("work") || lowerMessage.Contains("job"))
            {
                return "Work can be challenging but also rewarding! 💼 What do you do? I'd love to hear about your profession!";
            }
            else if (lowerMessage.Contains("study") || lowerMessage.Contains("learn"))
            {
                return "Learning is a lifelong journey! 📚 What are you studying or interested in learning? I'm here to help with any questions!";
            }
            else if (lowerMessage.Contains("movie") || lowerMessage.Contains("film"))
            {
                return "Movies are fantastic! 🎬 What's your favorite genre? I'd love to hear about the films you enjoy!";
            }
            else if (lowerMessage.Contains("book") || lowerMessage.Contains("read"))
            {
                return "Reading is such a wonderful hobby! 📖 What kind of books do you enjoy? I'd love to hear about your favorite authors!";
            }
            else if (lowerMessage.Contains("travel") || lowerMessage.Contains("vacation"))
            {
                return "Traveling is so exciting! ✈️ Where have you been or where would you like to go? I'd love to hear about your adventures!";
            }
            else if (lowerMessage.Contains("sport") || lowerMessage.Contains("game"))
            {
                return "Sports are great for staying active and having fun! ⚽ What sports do you enjoy playing or watching?";
            }
            else if (lowerMessage.Contains("family") || lowerMessage.Contains("friend"))
            {
                return "Family and friends are so important! 👨‍👩‍👧‍👦 How are your loved ones doing? I'd love to hear about them!";
            }
            else if (lowerMessage.Contains("dream") || lowerMessage.Contains("goal"))
            {
                return "Dreams and goals give us direction in life! 🌟 What are some of your dreams or goals? I'd love to hear about them!";
            }
            else
            {
                var responses = new[]
                {
                    "That's really interesting! Tell me more about that! 🤔",
                    "I'd love to hear more about your thoughts on this! 💭",
                    "That's a great point! What made you think about that? 🎯",
                    "I'm curious to know more! Can you elaborate? 🔍",
                    "That sounds fascinating! I'd like to understand better! ✨",
                    "That's intriguing! I'd love to hear more details! 🌟",
                    "That's a wonderful topic! Tell me more! 💫",
                    "I'm really interested in what you're saying! Can you share more? 🎨"
                };
                return responses[new Random().Next(responses.Length)];
            }
        }
    }
} 