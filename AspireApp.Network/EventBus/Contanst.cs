namespace EventBus
{
    public class Contanst
    {
        public const string CreatePostQueue = "create_post_queue";

        /// <summary>
        /// Khi add comment tai post service se ban event toi chat service de realtime comment
        /// </summary>
        public const string RealtimeCommentQueue = "realtime_comment_queue";
    }
}
