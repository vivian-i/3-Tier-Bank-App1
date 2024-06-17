using System;
using System.Web.Http;
using System.Drawing.Imaging;
using System.IO;
using APIClasses;
using WebApplication1.Models;
using System.Net.Http;
using DBInterface;
using System.ServiceModel;
using System.Web.Script.Serialization;
using System.Net;

namespace WebApplication1.Controllers
{
    /**
     * SearchController is a ASP.NET Web API controller class
     * It has a Post method that retrieves a DataIntermed object by the inputted SearchData value.
     */
    public class SearchController : ApiController
    {
        //private fields of LogHelper object to help log message to file
        LogHelper logHelper = new LogHelper();

        /**
         * This rest service retrieves a data intermed object.
         * If the http response is a success,  it returns the data intermed object in JSON.
         * Otherwise, a http response exception is thrown.
         */
        public DataIntermed Post([FromBody] SearchData value)
        {
            //create a new DataIntermed object
            DataIntermed dataIntermed = new DataIntermed();

            try
            {
                //create a new DataModel object
                DataModel dataModel = new DataModel();

                //calls GetMatchingLastName method
                dataModel.GetMatchingLastName(value.searchStr, out var acctNo, out var pin, out var bal, out var fName, out var lName, out var profilePic);
                //set the dataIntermed fields
                dataIntermed.fname = fName;
                dataIntermed.lname = lName;
                dataIntermed.bal = bal;
                dataIntermed.acct = acctNo;
                dataIntermed.pin = pin;

                //if profilePic is null, set the data intermed profile picture fields to null
                if (profilePic == null)
                {
                    //set the data intermed profile picture fields to null
                    dataIntermed.profilePic = null;
                }
                //if profilePic is not null, set the data intermed profile picture fields to its image
                else
                {
                    //convert Bitmap to base64
                    MemoryStream ms = new MemoryStream();
                    profilePic.Save(ms, ImageFormat.Jpeg);
                    byte[] byteImage = ms.ToArray();
                    //get Base64
                    var SigBase64 = Convert.ToBase64String(byteImage);

                    //set the dataIntermed profile pic fields
                    dataIntermed.profilePic = SigBase64;
                }
            }

            //catch the custom made Fault Exception which is InvalidTypeFault
            catch (FaultException<InvalidTypeFault> exception)
            {
                //create an error response
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(exception.Detail.Description);
                //log message to file
                logHelper.log($"[ERROR] Get() - GetMatchingLastName is NOT successfuly called. Last name {value.searchStr} is invalid. Http response exception is thrown.");
                //throw a http response exception
                throw new HttpResponseException(httpResponseMessage);
            }
            //catch the custom made Fault Exception which is NoMatchFault
            catch (FaultException<NoMatchFault> exception)
            {
                //create an error response
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                httpResponseMessage.Content = new StringContent(exception.Detail.Description);
                //log message to file
                logHelper.log($"[ERROR] Get() - GetMatchingLastName is NOT successfuly called. Last name {value.searchStr} has no match found. Http response exception is thrown.");
                //throw a http response exception
                throw new HttpResponseException(httpResponseMessage);
            }
            //catch exception and throw a http response exception
            catch (Exception exception)
            {
                //create an error response
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(exception.Message);
                //log message to file
                logHelper.log($"[ERROR] Get() - GetMatchingLastName is NOT successfuly called. Http response exception is thrown.");
                //throw a http response exception
                throw new HttpResponseException(httpResponseMessage);
            }

            //log message to file
            logHelper.log($"[INFO] Post() - GetMatchingLastName is successfuly called. Last name {value.searchStr} has an account number of {dataIntermed.acct} which is {dataIntermed.fname} {dataIntermed.lname}'s.");

            //return the data intermed object
            return dataIntermed;
        }
    }
}