using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;

namespace slackMessages.Models
{
    public class ScheduledMsgDTO : MessageDTO
    {
        [Required]
        public DateTime ScheduledDate { get; set; }
    }
}
