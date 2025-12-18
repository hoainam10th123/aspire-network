

namespace EventBus.Event
{
    public class AddComment
    {
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
