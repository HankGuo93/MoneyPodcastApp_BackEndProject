using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class UserRSSModel
    {
        [DataMember(Name = "usersId")]
        public int usersId { get; set; }

        [DataMember(Name = "podcastRSSId")]
        public int podcastRSSId { get; set; }
    }
}
