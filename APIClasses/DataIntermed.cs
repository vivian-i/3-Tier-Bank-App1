namespace APIClasses
{
    /**
     * DataIntermed represent the templates for the .NET JSON serializers.
     * It is going to be the go-betweens the web data.
     * DataIntermed will represent the format of the data to be passed through from the APIClasses Biz tier to the GUI.
     * DataIntermed defines the data that has a balance, account, pin, first name, last name and profilePic.
     */
    public class DataIntermed
    {
        public int bal;
        public uint acct;
        public uint pin;
        public string fname;
        public string lname;
        public string profilePic;
    }
}
