using Microsoft.AspNetCore.Mvc;

namespace slackMessages.Repositories
{
    public interface IChannelRepository
    {
        Task<Boolean> ChannelExists(string channel);
        Task<IActionResult> GetChannels();
    }
}
