using System.ServiceModel;
using System.Drawing;
using System;
using DBLibrary;
using DBInterface;
using System.Runtime.Serialization;

namespace DataServerConsoleApp
{
    /**
     *  DataServer is a C# console application of the Data-Tier.
     *  It is an internal implementation of the interface (the Data Server interface, the .NET Remoting network interface)
     *  It contains a method of GetNumEntries GetValuesForEntry, GetMatchingLastName and Log.
     */
    //defining the behaviours of a service by ServiceBehavior, makes the service multi-threaded by ConcurrencyMode and allow management of the thread synchronisation
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    [Serializable()]
    internal class DataServer : DataServerInterface
    {
        //a private fields that help makes the Data Tier a singleton 
        private readonly DatabaseClass dbClass = DatabaseClass.Instance;
        
        //public constructor
        public DataServer()
        {
            //create a new DatabaseClass
            dbClass = new DatabaseClass();
        }

        /**
         * GetNumEntries method retrieve the total number of record from the Database class
         * GetNumEntries method returns an int.
         */
        public int GetNumEntries()
        {
            //return DatabaseClass GetNumRecords method 
            return dbClass.GetNumRecords();
        }

        /**
         * GetValuesForEntry method retrieve values for an entry searched by the index inputted.
         * GetValuesForEntry method returns void.
         */
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic)
        {
            //if index is out-of-range, throw a new FaultException of the custom made fault which is ArgumentOutOfRangeFault
            if (index < 0 || index >= dbClass.GetNumRecords())
            {
                //write description to console
                Console.WriteLine("Client tried to get a record that was out of range.");

                //use the custom made fault
                ArgumentOutOfRangeFault fault = new ArgumentOutOfRangeFault();
                //define the fault fields
                fault.Message = "index is out of range.";
                fault.Description = "The index entered is out of range! Index starts from 0!";

                //throw a new FaultException of the custom made fault which is ArgumentOutOfRangeFault
                throw new FaultException<ArgumentOutOfRangeFault>(fault, new FaultReason(fault.Message));
            }

            //get the method and set it
            acctNo = dbClass.GetAcctNoByIndex(index);
            pin = dbClass.GetPINByIndex(index);
            bal = dbClass.GetBalanceByIndex(index);
            fName = dbClass.GetFirstNameByIndex(index);
            lName = dbClass.GetLastNameByIndex(index);
            profilePic = new Bitmap(dbClass.GetProfilePic(index));
        }

        /**
         * GetMatchingLastName method take in a string and return the contents of the first record that has a last name matching the string.
         * GetMatchingLastName method returns void.
         */
        public void GetMatchingLastName(string lastName, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap profilePic)
        {
            //set default values
            acctNo = 0;
            pin = 0;
            bal = 0;
            fName = "";
            lName = "";
            profilePic = null;

            //create a bool true to check if last name is a string
            bool islNameAllLetters = true;
            //check if the inputted last name is of the correct type. Only letters in last name check
            foreach (char c in lastName)
            {
                //if not all of the inputted last name is a letter, set bool to false
                if (!char.IsLetter(c))
                {
                    //set bool to false
                    islNameAllLetters = false;
                }
            }

            //exception if someone tries to search a number or special character, etc
            if (islNameAllLetters == false)
            {
                Console.WriteLine("Client tried to search a number or special character.");

                //use the custom made fault
                InvalidTypeFault fault = new InvalidTypeFault();
                //define the fault fields
                fault.Message = "invalid last name.";
                fault.Description = "Invalid last name entered! last name should be a String!";

                //throw a new FaultException of the custom made fault which is ArgumentOutOfRangeFault
                throw new FaultException<InvalidTypeFault>(fault, new FaultReason(fault.Message));
            }

            //bool to check if the first record is already found, initially set it to false
            bool isFirstRecordFound = false;

            //loop through all the records in the Database class
            for (int index = 0; index < dbClass.GetNumRecords(); index++)
            {
                //if the last name of the index is the same as what the user input in the parameter, add it to the list
                if (dbClass.GetLastNameByIndex(index).Equals(lastName) && isFirstRecordFound == false)
                {
                    //set first record is found to true
                    isFirstRecordFound = true;

                    //get the method and set it
                    acctNo = dbClass.GetAcctNoByIndex(index);
                    pin = dbClass.GetPINByIndex(index);
                    bal = dbClass.GetBalanceByIndex(index);
                    fName = dbClass.GetFirstNameByIndex(index);
                    lName = dbClass.GetLastNameByIndex(index);
                    profilePic = dbClass.GetProfilePic(index);
                }
            }

            //exception if someone searches a name with no match
            if (acctNo == 0 && pin == 0 && bal == 0 && fName.Equals("") && lName.Equals("") && profilePic == null)
            {
                Console.WriteLine("Client tried to get a record by the last name that has no match.");

                //use the custom made fault
                NoMatchFault fault = new NoMatchFault();
                //define the fault fields
                fault.Message = "no match found.";
                fault.Description = "The last name entered has no match found!";

                //throw a new FaultException of the custom made fault which is ArgumentOutOfRangeFault
                throw new FaultException<NoMatchFault>(fault, new FaultReason(fault.Description));
            }
        }
    }
}
