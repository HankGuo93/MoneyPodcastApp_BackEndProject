using System.Runtime.Serialization;

namespace podcast_project.Models
{
    public class errorcode
    {
        public static int successed = 200;
        public static int internalError = 500;
        public static int notFound = 501;
        public static int alreadyExist = 502;
        public static int formatError = 503;
    }
    public class JsonModel<T>
    {
        [DataMember(Name = "errorcode")]
        public int errorcode { get; set; }

        [DataMember(Name = "msg")]
        public T Msg { get; set; }
    }
}
