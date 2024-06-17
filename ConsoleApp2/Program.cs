using System;
using System.ServiceModel;
using DBInterface;

namespace BusinessServerConsoleApp
{
    /**
     *  This is a C# console application of the DB Server class.
     *  It is a Business-Tier.
     */
    class Program
    {
        static void Main(string[] args)
        {
            //write description in the console
            Console.WriteLine("This is my console application of my business-tier server. It is currently running!");
            //This is the actual host service system
            ServiceHost host;
            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(BusinessServer));
            //present the publicly accessible interface to the client. It tells .net to accept on any interface, use port 8200 and service name of BusinessService.
            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://0.0.0.0:8200/BusinessService");
            //And open the host for business!
            host.Open();
            //write description in the console
            Console.WriteLine("System Online");
            Console.ReadLine();
            //close the host
            host.Close();
        }
    }
}
