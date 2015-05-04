using System.Runtime.Serialization;

namespace NorthwindInterfaces.Exceptions
{
    [DataContract]
    public class InvalidOrderChangeException
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
