using BankBusinessTierWebApp.Models;
using DBInterface;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.ServiceModel;

namespace BusinessServerConsoleApp
{
    /**
     *  ServicePublishing is a C# console application of the Business-Tier.
     *  It is an internal implementation of the interface (the Business Server interface)
     *  It contains a method of GetNumEntries GetValuesForEntry, GetMatchingLastName and Log.
     */
    //defining the behaviours of a service by ServiceBehavior, makes the service multi-threaded by ConcurrencyMode and allow management of the thread synchronisation
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class BusinessServer : BusinessServerInterface
    {
        //public fields of DataServerInterface
        public DataServerInterface dataServer;
        //declare a static variable of LogNumber
        public static uint LogNumber;
        //private fields of LogHelper object to help log message to file
        LogHelper logHelper = new LogHelper();

        //public constructor
        public BusinessServer()
        {
            //represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //set the url
            string URL = "net.tcp://localhost:8100/DataService";
            //This is a factory that generates remote connections to our remote class which hides the RPC stuff. ConsoleApp1 namespace is the server prog
            ChannelFactory<DataServerInterface> dataServerFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            //create the channel
            dataServer = dataServerFactory.CreateChannel();

            //set log number to 0, the initial value
            LogNumber = 0;
        }

        /**
         * GetNumEntries method calls the data-tier associated method and log message to file
         * GetNumEntries method returns an int.
         */
        public int GetNumEntries()
        {
            //calls data-tier method
            int numEntries = dataServer.GetNumEntries();
            //calls log method to log message to file
            Log($"[INFO] GetNumEntries() - successfuly called. It has {numEntries} items.", true);

            //return GetNumEntries functions of the Data tier 
            return numEntries;
        }

        /**
         * GetValuesForEntry method calls the data-tier associated method and log message to file
         * GetValuesForEntry method returns void.
         */
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic)
        {
            //if index is out-of-range, throw a custom FaultExecption of ArgumentOutOfRangeFault.
            if (index < 0 || index >= dataServer.GetNumEntries())
            {
                //use the custom made ArgumentOutOfRangeFault fault
                ArgumentOutOfRangeFault fault = new ArgumentOutOfRangeFault();

                //define the fault fields
                fault.Message = "Index is out of range.";
                fault.Description = "The index entered is out of range! Index starts from 0!";

                //call log method
                Log($"[ERROR] GetValuesForEntry() - index of {index} is out of range. Throwing ArgumentOutOfRangeFault custom Fault Exception.", false);

                //throw a new FaultException of the custom made fault which is ArgumentOutOfRangeFault
                throw new FaultException<ArgumentOutOfRangeFault>(fault);
            }

            //call GetValuesForEntry functions of the Data tier 
            dataServer.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out profilePic);

            //call log method
            Log($"[INFO] GetValuesForEntry() - index of {index} has just been called succesfully. It has an account number of {acctNo} which is {fName} {lName}'s.", true);
        }

        /**
         * GetMatchingLastName method calls the data-tier associated method and log message to file
         * GetMatchingLastName method returns void.
         */
        public void GetMatchingLastName(string lastName, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic)
        {
            //if someone tries to search a number or special character, etc, throw an custom FaultExecption of InvalidTypeFault.
            if (int.TryParse(lastName, out var s))
            {
                //use the custom made InvalidTypeFault fault
                InvalidTypeFault fault = new InvalidTypeFault();
                //define the fault fields
                fault.Message = "invalid last name.";
                fault.Description = "Invalid last name! The last name entered should be a String!";

                //call log method
                Log($"[ERROR] GetMatchingLastName() - last name {lastName} is invalid. Throwing InvalidTypeFault custom Fault Exception.", false);

                //throw a new FaultException of the custom made fault which is ArgumentOutOfRangeFault
                throw new FaultException<InvalidTypeFault>(fault);
            }

            //call GetMatchingLastName functions of the Data tier 
            dataServer.GetMatchingLastName(lastName, out acctNo, out pin, out bal, out fName, out lName, out profilePic);

            //if someone tries to searches a name with no match, throw an custom FaultExecption of NoMatchFault.
            if (acctNo == 0 && pin == 0 && bal == 0 && fName.Equals("") && lName.Equals("") && profilePic == null)
            {
                //use the custom made NoMatchFault fault
                NoMatchFault fault = new NoMatchFault();
                //define the fault fields
                fault.Message = "no match found.";
                fault.Description = "The last name entered has no match found!";

                //call log method
                Log($"[ERROR] GetMatchingLastName() - last name {lastName} has no match found. Throwing NoMatchFault custom Fault Exception.", false);

                //throw a new FaultException of the custom made fault which is NoMatchFault
                throw new FaultException<NoMatchFault>(fault);
            }

            //call log method
            Log($"[INFO] GetMatchingLastName() - last name {lastName} has just been called succesfully. It has an account number of {acctNo} which is {fName} {lName}'s.", true);
        }

        /**
         * Log method is an access log that keep track of how many log-able tasks have been performed.
         * It also log the message to a file
         * Log method returns void.
         */
        //auto-thread-sync tools that tell .NET to only allow one thread at a time to run this function, and thus automatically sync it for you
        [MethodImpl(MethodImplOptions.Synchronized)]
        void Log(string logString, bool isSuccess)
        {
            //if the bool is true or isSuccessful, increase the task number
            if (isSuccess == true)
            {
                //increase task number
                LogNumber++;
            }
            //log message to a file
            logHelper.log(logString + "- succesful tasks performed: " + LogNumber);
            //log message to console
            Console.WriteLine($"{logString} - succesful tasks performed: {LogNumber}");
        }
    }
}
