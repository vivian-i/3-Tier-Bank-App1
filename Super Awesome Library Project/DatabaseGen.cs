using System;
using System.Drawing;
using System.Collections.Generic;

namespace DBLibrary
{
    /**
     * DatabaseGen is a pseudo-random generator of database entries
     */
    internal class DatabaseGen
    {
        //private readonly fields
        private readonly Random rand = new Random();
        private readonly string[] fNameList = {
            "Emily", "Natasha", "John", "Jane", "Michael", "William", "David", "Stefan", "Nelson", "Richard", "Charlie", "Mary", "Linda", "Susan", "Jessica", "Kathleen", "Anna"
        };
        private readonly string[] lNameList = {
            "Smith", "Johnson", "Williams", "Jones", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "Citizen", "Doe"
        };
        private readonly List<Bitmap> profilePicList;

        //default constructor
        public DatabaseGen()
        {
            //creates a new bitmap list
            profilePicList = new List<Bitmap>();
            //generate a few really basic icons
            for (var i = 0; i < 10; i++)
            {
                var image = new Bitmap(64, 64);
                for (var x = 0; x < 64; x++)
                {
                    for (var y = 0; y < 64; y++)
                    {
                        //set the pixel to the image
                        image.SetPixel(x, y, Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)));
                    }
                }
                //add image to the list
                profilePicList.Add(image);
            }
        }

        //GetFirstName method gets the first name by random name in the list and return a string
        private string GetFirstName() => fNameList[rand.Next(fNameList.Length)];

        //GetFirstName method gets the first name by random name in the list and return a string
        private string GetLastName() => lNameList[rand.Next(lNameList.Length)];

        //GetPin method gets the pin by random number and return a uint
        private uint GetPin() => (uint)rand.Next(9999);

        //GetAcctNo method gets the account number by random number and return a uint
        private uint GetAcctNo() => (uint)rand.Next(100000000, 999999999);

        //GetBalance method gets the balance by random number and return a int
        private int GetBalance() => rand.Next(0, 10000);

        //GetProfilePic method gets the image of the profile picture by random image created and return a Bitmap
        private Bitmap GetProfilePic() => profilePicList[rand.Next(profilePicList.Count)];

        //NumOfEntries method gets the number of entries by random number and return a int
        public int NumOfEntries() => rand.Next(100000, 999999);

        /**
         * GetNextAccount method lets someone request a record.
         * The 'out' parameter is used for multiple returns.
         */
        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance, out Bitmap profilePic)
        {
            //get the fields and set it
            pin = GetPin();
            acctNo = GetAcctNo();
            firstName = GetFirstName();
            lastName = GetLastName();
            balance = GetBalance();
            profilePic = GetProfilePic();
        }
    }
}
