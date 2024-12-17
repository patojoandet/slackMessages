using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using slackMessages.Models;
using slackMessages.Repositories;
using System.Text.Json;

namespace slackMessages.Controllers
{
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelRepository _channelRepository;
        public ChannelsController(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        [HttpGet("channels")]
        public async Task<ActionResult> ListChannels()
        {
            var channels = await _channelRepository.GetChannels();
            if (((IStatusCodeActionResult)channels).StatusCode == 200) { 
                return Ok(channels);
            }
            return BadRequest();
            
        }
    }
}
