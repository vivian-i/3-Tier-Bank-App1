using System;
using System.Runtime.Serialization;

namespace DBInterface
{
    //use Serializable to serialize exceptions for web service
    [Serializable()]
    //use DataContract for custom made fault for no match fault
    [DataContract]
    /**
     *  NoMatchFault is a custom made fault class for no match fault.
     *  It has a message and description method.
     */
    public class NoMatchFault
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
