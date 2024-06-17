using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DBLibrary
{
    /**
     * DatabaseClass is the publicly accessible object that will define the database.
     */
    public class DatabaseClass
    {
        //fields
        private readonly List<DataStruct> dbList; //holds a list of database storage classes
        public static DatabaseClass Instance { get; } = new DatabaseClass();
        static DatabaseClass() { }

        //public contructor
        public DatabaseClass()
        {
            // Create the fake values for our fake database
            dbList = new List<DataStruct>();
            //creates new DatabaseGen
            var generator = new DatabaseGen();
            //loop through the generator number of entries
            for (var i = 0; i < generator.NumOfEntries(); i++)
            {
                //creates new DataStruct
                var temp = new DataStruct();
                //get the next account
                generator.GetNextAccount(out temp.pin, out temp.acctNo, out temp.firstName, out temp.lastName, out temp.balance, out temp.profilePic);
                //add the DataStruct to the list
                dbList.Add(temp);
            }
        }

        /**
         *  GetAcctNoByIndex return the account number by the index.
         *  It returns an uint.
         */
        public uint GetAcctNoByIndex(int index)
        {
            //return the account number by the index
            return dbList.ElementAt(index).acctNo;
        }

        /**
         *  GetPINByIndex return the pin by the index.
         *  It returns an uint.
         */
        public uint GetPINByIndex(int index)
        {
            //return the pin by the index
            return dbList.ElementAt(index).pin;
        }

        /**
         *  GetFirstNameByIndex return the first name by the index.
         *  It returns a string.
         */
        public string GetFirstNameByIndex(int index)
        {
            //return the first name by the index
            return dbList.ElementAt(index).firstName;
        }

        /**
         *  GetLastNameByIndex return the last name by the index.
         *  It returns a string.
         */
        public string GetLastNameByIndex(int index)
        {
            //return the last name by the index
            return dbList.ElementAt(index).lastName;
        }

        /**
         *  GetBalanceByIndex return balance by the index.
         *  It returns an int.
         */
        public int GetBalanceByIndex(int index)
        {
            //return the balance by the index
            return dbList.ElementAt(index).balance;
        }

        /**
         *  GetProfilePic return the bitmap profile picture by the index.
         *  It returns a Bitmap.
         */
        public Bitmap GetProfilePic(int index)
        {
            //clone the bitmap profile picture before assigning it to an out variable,
            //so that it can be accessed by an index in the gui more than once without an exception occured.
            //beacause if not, c# will try to use a reference to the bitmap, instead of the bitmap itself.
            //return the bitmap profile picture by the index.
            return (Bitmap)dbList.ElementAt(index).profilePic.Clone();
        }

        /**
         *  GetNumRecords return the total number in the list.
         *  It returns an int.
         */
        public int GetNumRecords()
        {
            //return the total number in the list
            return dbList.Count();
        }
    }
}
