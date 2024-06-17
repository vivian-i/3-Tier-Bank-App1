using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;
using RestSharp;
using DBInterface;
using APIClasses;

namespace ClientWpfApp
{
    //use Delegate for load and search function
    public delegate IRestResponse SearchByLNameDelegate(string inputLName);
    public delegate IRestResponse LoadByIndexDelegate(int index);
    /**
     *  MainWindow is a public partial class and is a user interface
     *  It is an Interaction logic for MainWindow.xaml
     */
    public partial class MainWindow : Window
    {
        //private fields
        private RestClient client;

        //the MainWindow
        public MainWindow()
        {
            //Set up the window, initialize the component
            InitializeComponent();

            //hide the progress bar
            AsyncProgressBar.Visibility = Visibility.Hidden;

            //set the base url
            string URL = "https://localhost:44322/";
            //use RestClient and set the URL
            client = new RestClient(URL);
            //set up and call the API method
            RestRequest request = new RestRequest("api/values");
            //use IRestResponse and set the request in the client get method
            IRestResponse resp = client.Get(request);

            try
            {
                //if response is succesful, set the total number of the items 
                if (resp.IsSuccessful)
                {
                    //set the rest response numOfThings content as the TotalItemLabel text 
                    TotalItemBox.Text = resp.Content;
                }
                //if response not is succesful, show error message to user
                else
                {
                    //set the total item text to null
                    TotalItemBox.Text = "";
                    //show message to user in the gui
                    MessageBox.Show("Error - " + resp.Content);
                    Console.WriteLine("Error - " + resp.Content);
                }
            }
            //catch the EndpointNotFoundException, if no end point or server is found
            catch (EndpointNotFoundException)
            {
                //show message to user in the gui
                MessageBox.Show("No end point found.");
                Console.WriteLine("No end point found.");
            }
            //catch other exception
            catch (Exception exception)
            {
                //show message to user in the gui
                MessageBox.Show(exception.Message);
                Console.WriteLine(exception.Message);
            }
        }

        /**
         *  method for when go button is clicked.
         *  If the index entered is an integer, do load and set the values in the GUI asynchronously.
         *  Otherwise, show message to the user.
         */
        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            //check if the inputted values is of the correct type
            bool isIndexAnInt = int.TryParse(IndexBox.Text, out int index);

            //if index is not an integer, show error message to user in the gui
            if (!isIndexAnInt)
            {
                //set the gui values to null
                FNameBox.Text = "";
                LNameBox.Text = "";
                AcctNoBox.Text = "";
                PinBox.Text = "";
                BalanceBox.Text = "";

                //show message to user
                MessageBox.Show($"\"{IndexBox.Text}\" is not a valid integer.");
                Console.WriteLine($"\"{IndexBox.Text}\" is not a valid integer.");
            }
            //if index is an integer, do load the entry data by the index asynchronously
            else
            {
                //disable buttons and text boxes
                disableBtn();
                //declare the delegate and async callback
                LoadByIndexDelegate loadDel;
                AsyncCallback callbackDel;

                //point searchDel to the search async method
                loadDel = LoadByIndexAsync;
                //point callback delegate at callback function
                callbackDel = this.OnLoadByIndexComplete;
                //start worker thread which is the async callback that is defined by .NET for BeginInvoke
                loadDel.BeginInvoke(index, callbackDel, null);
                //write description to console
                Console.WriteLine("waiting for (Load by Index) completion...");
            }
        }

        /**
        * LoadByIndexAsync is the callback function.
        * It is used to load the entry data by the index.
        * It will be use for the asynchronous call.
        */
        private IRestResponse LoadByIndexAsync(int index)
        {
            //set up and call the API method
            RestRequest request = new RestRequest("api/getvalues/" + index.ToString());
            IRestResponse resp = client.Get(request);
            //return the rest response
            return resp;
        }

        /**
         * OnLoadByIndexComplete method contains a parameter of IAsyncResult.
         * It retrieves the result of the data entry.
         * This method is called by AsyncCallback callBackDel.
         * Also, must be the same signature as AsyncCallback.
         *
         * By using Delegate and Async, my GUI is now put into a 'waiting state'.
         * waiting state means still responsive, but doesn't let user do anything.
         * After that, the code wake it up using the completion callback.
         */
        public void OnLoadByIndexComplete(IAsyncResult asyncResult)
        {
            //declare the load delegate 
            LoadByIndexDelegate loadDel;
            //use AsyncResult so that we can get the AsyncDelegate
            AsyncResult asyncObj = (AsyncResult)asyncResult;

            //must no call EndInvoke more than once
            if (asyncObj.EndInvokeCalled == false)
            {
                //gain access to delegate for EndInvoke
                loadDel = (LoadByIndexDelegate)asyncObj.AsyncDelegate;
                //retrieve the ref or out params
                IRestResponse data = loadDel.EndInvoke(asyncObj);
                //race condition with result output in Main()
                Console.WriteLine("(Load By Index) race condition with result output in Main().");
                //set the gui values
                setGuiValues(data);
            }

            //clean up
            asyncObj.AsyncWaitHandle.Close();
        }

        /**
         *  method for when search button is clicked.
         *  If search button is clicked, do search, load and set the values in the GUI asynchronously. 
         */
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            //disable buttons and text boxes
            disableBtn();
            //declare the delegate and async callback
            SearchByLNameDelegate searchDel;
            AsyncCallback callbackDel;

            //point searchDel to the search async method
            searchDel = SearchByLNameAsync;
            //point callback delegate at callback function
            callbackDel = this.OnSearchByLNameComplete;
            //start worker thread which is the async callback that is defined by .NET for BeginInvoke
            searchDel.BeginInvoke(InputLName.Text, callbackDel, null);

            Console.WriteLine("waiting for (Search by Last Name) completion...");
        }

        /**
         * SearchByLNameAsync is the callback function.
         * It is used to search the entry data by the last name.
         * It will be use for the asynchronous call.
         */
        private IRestResponse SearchByLNameAsync(string inputLName)
        {
            //creates a SearchData object
            SearchData mySearch = new SearchData();
            //set the search string of SearchData as the InputLName text
            mySearch.searchStr = inputLName;
            //Build a request with the json in the body
            RestRequest request = new RestRequest("api/search/");
            request.AddJsonBody(mySearch);
            //Do the request
            IRestResponse resp = client.Post(request);
            //return the response
            return resp;
        }

        /**
         * OnSearchByLNameComplete method contains a parameter of IAsyncResult.
         * It retrieves the result of the data entry.
         * This method is called by AsyncCallback callBackDel.
         * Also, must be the same signature as AsyncCallback.
         *
         * By using Delegate and Async, my GUI is now put into a 'waiting state'.
         * waiting state means still responsive, but doesn't let user do anything.
         * After that, the code wake it up using the completion callback.
         */
        public void OnSearchByLNameComplete(IAsyncResult asyncResult)
        {
            //declare the search delegate
            SearchByLNameDelegate searchDel;
            //use AsyncResult so that we can get the AsuncDelegate
            AsyncResult asyncObj = (AsyncResult)asyncResult;

            //must no call EndInvoke more than once
            if (asyncObj.EndInvokeCalled == false)
            {
                //gain access to delegate for EndInvoke
                searchDel = (SearchByLNameDelegate)asyncObj.AsyncDelegate;
                //retrieve the ref or out params
                IRestResponse resp = searchDel.EndInvoke(asyncObj);
                //race condition with result output in Main()
                Console.WriteLine("(Search by Last Name) race condition with result output in Main().");
                //set gui values
                setGuiValues(resp);
            }

            //clean up
            asyncObj.AsyncWaitHandle.Close();
        }

        /**
         * setGuiValues method contains a DataIntermed parameter.
         * It sets the gui values and re-enable the buttons and text boxes.
         */
        public void setGuiValues(IRestResponse resp)
        {
            //invoke
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    //if the response is succesful, do deserialize the object and set the gui values
                    if (resp.IsSuccessful)
                    {
                        //deserialize the data intermed object 
                        DataIntermed data = JsonConvert.DeserializeObject<DataIntermed>(resp.Content);

                        //if dataIntermed is null, show message to user in the gui
                        if (data == null)
                        {
                            //show message to user in the gui
                            MessageBox.Show("the data is empty or null. maybe you forget to start the data or web app");
                        }
                        else
                        {
                            //set the gui values
                            FNameBox.Text = data.fname;
                            LNameBox.Text = data.lname;
                            AcctNoBox.Text = data.acct.ToString();
                            PinBox.Text = data.pin.ToString("D4");
                            BalanceBox.Text = data.bal.ToString();

                            //check if profile picture is not null
                            if (data.profilePic != null)
                            {
                                //decode base64 to bitmap
                                Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(data.profilePic));
                                System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
                                Bitmap bitImage = new Bitmap((Bitmap)Image.FromStream(streamBitmap));

                                //load the profile picture in the image and convert to image source
                                ProfilePicImg.Source = Imaging.CreateBitmapSourceFromHBitmap(bitImage.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                //dispose the bitImage
                                bitImage.Dispose();
                            }
                            //if profile picture is null, set the profile picture image source to null
                            else
                            {
                                //set the profile picture image source to null
                                ProfilePicImg.Source = null;
                            }
                        }
                    }
                    //if the response is not succesful, show error message to user in the gui
                    else
                    {
                        //set the gui values to null
                        FNameBox.Text = "";
                        LNameBox.Text = "";
                        AcctNoBox.Text = "";
                        PinBox.Text = "";
                        BalanceBox.Text = "";

                        //show message to user in the gui
                        MessageBox.Show("Error - " + resp.Content);
                        Console.WriteLine("Error - " + resp.Content);
                    }
                }
                catch (JsonReaderException exception)
                {
                    //show message to user in the gui
                    MessageBox.Show(exception.Message);
                    Console.WriteLine(exception.Message);
                }
                // catch null reference from the JSON object.
                catch (NullReferenceException exception)
                {
                    //show message to user in the gui
                    MessageBox.Show(exception.Message);
                    Console.WriteLine(exception.Message);
                }
                //catch the fault in the communication object
                catch (CommunicationObjectFaultedException exception)
                {
                    //show message to user in the gui
                    MessageBox.Show(exception.Message);
                    Console.WriteLine(exception.Message);
                }
                //catch the other unrecognized fault exception
                catch (FaultException exception)
                {
                    //show message to user in the gui
                    MessageBox.Show(exception.Message);
                    Console.WriteLine(exception.Message);
                }
                //catch the standard communication fault
                catch (CommunicationException exception)
                {
                    //show message to user in the gui
                    MessageBox.Show(exception.Message);
                    Console.WriteLine(exception.Message);
                }
                //catch the other exception
                catch (Exception exception)
                {
                    //show message to user in the gui
                    MessageBox.Show(exception.Message);
                    Console.WriteLine(exception.Message);
                }

                //enable buttons and text boxes
                enableBtn();
            });
        }

        /**
         * FixBase64ForImage method takes a string parameter.
         * It Fixes the base64 for the inputted image
         * FixBase64ForImage method returns a string.
         */
        public string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }

        /**
        * disableBtn method disable all the buttons.
        * It also set the boxes to read only and set the progress bar.
        */
        public void disableBtn()
        {
            //set the boxes to read only
            IndexBox.IsReadOnly = true;
            InputLName.IsReadOnly = true;

            //disable GUI buttons 
            GoBtn.IsEnabled = false;
            SearchBtn.IsEnabled = false;

            //show the progress bar
            AsyncProgressBar.Visibility = Visibility.Visible;

            //write description to console
            Console.WriteLine("text boxes isReadOnly and buttons are disabled.");
        }

        /**
         * enableBtn method enable all the buttons.
         * It also set the boxes to not read only and set the progress bar.
         */
        public void enableBtn()
        {
            //set the boxes to not read only
            IndexBox.IsReadOnly = false;
            InputLName.IsReadOnly = false;

            //enable GUI buttons 
            GoBtn.IsEnabled = true;
            SearchBtn.IsEnabled = true;

            //hide the progress bar
            AsyncProgressBar.Visibility = Visibility.Hidden;

            //write description to console
            Console.WriteLine("text boxes can be used and buttons are re-enabled.");
        }
    }
}
