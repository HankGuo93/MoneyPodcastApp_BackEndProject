using System;
using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class PodcastRSSModel
    {
        [DataMember(Name = "podcastRSSId")]
        public int podcastRSSId { get; set; }

        [DataMember(Name = "author")]
        public string author { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "mp3Link")]
        public string mp3Link { get; set; }

        [DataMember(Name = "description")]
        public string description { get; set; }

        [DataMember(Name = "duration")]
        public int duration { get; set; }

        [DataMember(Name = "imgLink")]
        public string imgLink { get; set; }

        [DataMember(Name = "pubdate")]
        public DateTime pubdate { get; set; }

        [DataMember(Name = "timestamp")]
        public int timestamp { get; set; }

        [DataMember(Name = "isCollect")]
        public Boolean isCollect { get; set; }
        [DataMember(Name = "isToListen")]
        public Boolean isToListen { get; set; }
    }
}
