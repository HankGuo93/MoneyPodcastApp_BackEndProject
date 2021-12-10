using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class CommentModel
    {
        [DataMember(Name = "podcastRSSId")]
        public int podcastRSSId { get; set; }

        [DataMember(Name = "content")]
        public string content { get; set; }
    }

    public class CommentContentModel
    {
        [DataMember(Name = "commentsId")]
        public int commentsId { get; set; }

        [DataMember(Name = "content")]
        public string content { get; set; }

        [DataMember(Name = "likeCount")]
        public int likeCount { get; set; }
    }

    public class CommentUserModel
    {
        [DataMember(Name = "commentsId")]
        public int commentsId { get; set; }

        [DataMember(Name = "usersId")]
        public int usersId { get; set; }
    }
}
