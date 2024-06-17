using System;
using System.Runtime.Serialization;

namespace DBInterface
{
    //use DataContract for custom made fault for out of range
    [DataContract]
    /**
     *  ArgumentOutOfRangeFault is a custom made fault class for out of range fault.
     *  It has a message and description method.
     */
    public class ArgumentOutOfRangeFault
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
