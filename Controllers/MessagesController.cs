using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using slackMessages.Models;
using System.Text.Json;
using System.Text;
using slackMessages.Repositories;

namespace slackMessages.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SlackSettings _slackSettings;
        private readonly string _postMessageUrl = "https://slack.com/api/chat.postMessage";
        private readonly string _scheduleMessageUrl = "https://slack.com/api/chat.scheduleMessage";
        private readonly HttpClient _client;
        private readonly IChannelRepository _channelRepository;


        public MessageController(IHttpClientFactory httpClientFactory, IOptions<SlackSettings> slackSettings, IChannelRepository channelRepository)
        {
            _httpClientFactory = httpClientFactory;
            _slackSettings = slackSettings.Value;
            _client = _httpClientFactory.CreateClient();
            _channelRepository = channelRepository;
        }

        [HttpPost("send")]
        public async Task<ActionResult> SendMessage(MessageDTO message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _channelRepository.ChannelExists(message.Channel))
            {
                return BadRequest("Channel doesn't exists");
            }
            
            var payload = new
            {
                channel = message.Channel,
                text = message.Message
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _slackSettings.BotToken);


            var response = await _client.PostAsync(_postMessageUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("sent");
            }

            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }

        [HttpPost("schedule")]
        public async Task<ActionResult> ScheduleMessage(ScheduledMsgDTO scheduleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _channelRepository.ChannelExists(scheduleDTO.Channel))
            {
                return BadRequest("Channel doesn't exists");
            }

            var utcTime = DateTime.SpecifyKind(scheduleDTO.ScheduledDate, DateTimeKind.Local).ToUniversalTime();


            var payload = new
            {
                channel = scheduleDTO.Channel,
                text = scheduleDTO.Message,
                post_at = new DateTimeOffset(utcTime).ToUnixTimeSeconds()
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _slackSettings.BotToken);

            var response = await _client.PostAsync(_scheduleMessageUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Message scheduled");
            }

            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }
    }
}
