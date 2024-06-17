using System;
using System.Runtime.Serialization;

namespace DBInterface
{
    //use Serializable to serialize exceptions for web service
    [Serializable()]
    //use DataContract for custom made fault for invalid type
    [DataContract]
    /**
     *  InvalidTypeFault is a custom made fault class for invalid type.
     *  It has a message and description method.
     */
    public class InvalidTypeFault
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
