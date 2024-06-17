using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Drawing;
using DBInterface;

namespace WebApplication1.Models
{
    /**
      * DataModel is a public class that connects to the Data Tier via .NET remoting.
      * It allows for the usual services, such as GetNumEntries, GetValuesForEntry and GetMatchingLastName.
      */
    public class DataModel
    {
        //public field
        public DataServerInterface dataServer;

        //public constructor
        public DataModel()
        {
            //represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection
            string URL = "net.tcp://localhost:8100/DataService";
            //This is a factory that generates remote connections to our remote class which hides the RPC stuff
            ChannelFactory<DataServerInterface> dataServerFactory = new ChannelFactory<DataServerInterface>(tcp, URL);//ConsoleApp1 namespace == the server prog
            //create the channel
            dataServer = dataServerFactory.CreateChannel();
        }

        /**
        * GetNumEntries method return an int
        * It returns the Authenticator Service GetNumEntries method result
        */
        public int GetNumEntries()
        {
            //return GetNumEntries functions of the Data server 
            return dataServer.GetNumEntries();
        }

        /**
        * GetValuesForEntry method calls the Authenticator Service GetValuesForEntry method
        * It has an out param for multiple returns
        */
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic)
        {
            //call GetValuesForEntry functions of the Data server 
            dataServer.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out profilePic);
        }

        /**
        * GetMatchingLastName method calls the Authenticator Service GetMatchingLastName method
        * It has an out param for multiple returns
        */
        public void GetMatchingLastName(string lastName, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic)
        {
            //call GetMatchingLastName functions of the Data server 
            dataServer.GetMatchingLastName(lastName, out acctNo, out pin, out bal, out fName, out lName, out profilePic);
        }
    }
}