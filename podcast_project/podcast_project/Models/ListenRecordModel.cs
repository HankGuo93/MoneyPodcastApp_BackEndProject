using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class ListenRecordModel
    {
        [DataMember(Name = "usersId")]
        public int usersId { get; set; }

        [DataMember(Name = "podcastRSSId")]
        public int podcastRSSId { get; set; }

        [DataMember(Name = "timestamp")]
        public int timestamp { get; set; }
    }
}
