using System.ServiceModel;
using System.Drawing;

namespace DBInterface
{
    //ServiceContract makes this a service contract as it is a service interface
    [ServiceContract]
    /**
     *  DataServerInterface is used for the data-tier interface.
     *  It is the public interface for the .NET server, the .NET Remoting network interface.
     */
    public interface DataServerInterface
    {
        //Tag OperationContracts for this is a service function contracts
        [OperationContract]
        int GetNumEntries();

        //Tag OperationContracts for this is a service function contracts
        [OperationContract]
        //for index out of range. Use FaultContract for exception handling to cross the network boundary.
        [FaultContract(typeof(ArgumentOutOfRangeFault))]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic);

        //Tag OperationContracts for this is a service function contracts
        [OperationContract]
        //for no match fault if someone tries to searches a name with no match. Use FaultContract for exception handling to cross the network boundary.
        [FaultContract(typeof(NoMatchFault))]
        //for invalid type fault if someone tries to search a number or special character. Use FaultContract for exception handling to cross the network boundary.
        [FaultContract(typeof(InvalidTypeFault))]
        void GetMatchingLastName(string lastName, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic);
    }
}
