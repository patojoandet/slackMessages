using System.ComponentModel.DataAnnotations;

namespace slackMessages.Models
{
    public class MessageDTO
    {
        [Required]
        public string Channel { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
