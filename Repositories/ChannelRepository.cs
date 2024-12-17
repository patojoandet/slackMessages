
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using slackMessages.Models;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace slackMessages.Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SlackSettings _slackSettings;
        private readonly string _channelsUrl = "https://slack.com/api/conversations.list";
        private readonly HttpClient _client;

        public ChannelRepository(IHttpClientFactory httpClientFactory, IOptions<SlackSettings> slackSettings)
        {
            _httpClientFactory = httpClientFactory;
            _slackSettings = slackSettings.Value;
            _client = _httpClientFactory.CreateClient();
        }
        public async Task<Boolean> ChannelExists(string channel)
        {
            _client.DefaultRequestHeaders.Authorization =
               new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _slackSettings.BotToken);

            var response = await _client.GetAsync(_channelsUrl);

            var content = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);

            if (jsonResponse.TryGetProperty("channels", out var channels))
            {
                var channelExists = channels.EnumerateArray()
                                             .Any(ch => ch.GetProperty("id").GetString().Equals(channel));

                if (channelExists)
                    return true;
            }

            return false;
        }

        public async Task<IActionResult> GetChannels()
        {
            _client.DefaultRequestHeaders.Authorization =
              new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _slackSettings.BotToken);
            var response = await _client.GetAsync(_channelsUrl);

            var content = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);

            if (jsonResponse.TryGetProperty("ok", out var ok) && ok.GetBoolean())
            {
                return new OkObjectResult(jsonResponse);
            }
            
            return new BadRequestObjectResult(jsonResponse);
        }
    }
}
