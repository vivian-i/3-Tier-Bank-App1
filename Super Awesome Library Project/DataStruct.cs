using System.Drawing;

namespace DBLibrary
{
    /**
     * DataStruct is an internal class that store individual records in the database.
     * It contains an account number, pin, balance, first name, last name and its profile picture.
     */
    internal class DataStruct
    {
        //public fields
        public uint acctNo;
        public uint pin; 
        public int balance;
        public string firstName;
        public string lastName;
        public Bitmap profilePic;

        //public constructor
        public DataStruct()
        {
            acctNo = 0;
            pin = 0;
            balance = 0;
            firstName = "";
            lastName = "";
            profilePic = null;
        }
    }
}