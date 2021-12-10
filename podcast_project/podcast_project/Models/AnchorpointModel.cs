using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class AnchorpointModel
    {
        [DataMember(Name = "usersId")]
        public int usersId { get; set; }

        [DataMember(Name = "podcastRSSId")]
        public int podcastRSSId { get; set; }

        public int anchorTime { get; set; }

        [DataMember(Name = "content")]
        public string content { get; set; }
    }

    public class AnchorpointWithoutUserModel
    {
        [DataMember(Name = "anchorpointId")]
        public int anchorpointId { get; set; }

        [DataMember(Name = "anchorTime")]
        public int anchorTime { get; set; }

        [DataMember(Name = "content")]
        public string content { get; set; }
    }

    public class AnchorpointWithTitleModel
    {
        [DataMember(Name = "anchorpointId")]
        public int anchorpointId { get; set; }

        [DataMember(Name = "anchorTime")]
        public int anchorTime { get; set; }

        [DataMember(Name = "content")]
        public string content { get; set; }

        [DataMember(Name = "podcastRSSId")]
        public int podcastRSSId { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }
    }

    public class AnchorpointIdModel
    {
        [DataMember(Name = "anchorpointId")]
        public int anchorpointId { get; set; }
    }
}
