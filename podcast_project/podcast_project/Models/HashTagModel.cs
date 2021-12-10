using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class HashTagModel
    {
        [DataMember(Name = "hashTagId")]
        public int hashTagId { get; set; }

        [DataMember(Name = "hashTag")]
        public string hashTag { get; set; }
    }
}
