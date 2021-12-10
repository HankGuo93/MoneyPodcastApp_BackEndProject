using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class UserModel
    {
        [DataMember(Name = "usersId")]
        public int usersId { get; set; }

        [DataMember(Name = "IDFA")]
        public string IDFA { get; set; }
    }
    public class UserIdModel
    {
        [DataMember(Name = "usersId")]
        public int usersId { get; set; }
    }
    public class UserIdWithTokenModel
    {
        [DataMember(Name = "usersId")]
        public int usersId { get; set; }
        [DataMember(Name = "token")]
        public string token { get; set; }
    }

    public class IDFAdModel
    {
        [DataMember(Name = "IDFA")]
        public string IDFA { get; set; }
    }
}
