using System;
using System.Web.Http;
using System.Drawing.Imaging;
using APIClasses;
using System.IO;
using WebApplication1.Models;
using System.Net.Http;
using System.ServiceModel;
using DBInterface;
using System.Net;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace WebApplication1.Controllers
{
    /**
     * GetValuesController is a ASP.NET Web API controller class
     * It has a Get method that retrieves a DataIntermed object by the inputted id.
     */
    public class GetValuesController : ApiController
    {
        //private fields of LogHelper object to help log message to file
        LogHelper logHelper = new LogHelper();

        /**
         * This rest service retrieves a data intermed object.
         * If the http response is a success,  it returns the data intermed object in JSON.
         * Otherwise, a http response exception is thrown.
         */
        public DataIntermed Get(int id)
        {
            //create a new DataIntermed object
            DataIntermed dataIntermed = new DataIntermed();

            try
            {
                //create a new DataModel object
                DataModel dataModel = new DataModel();

                //calls GetValuesForEntry method 
                dataModel.GetValuesForEntry(id, out var acctNo, out var pin, out var bal, out var fName, out var lName, out var profilePic);
                //set the dataIntermed fields
                dataIntermed.fname = fName;
                dataIntermed.lname = lName;
                dataIntermed.bal = bal;
                dataIntermed.acct = acctNo;
                dataIntermed.pin = pin;

                //convert Bitmap to base64
                MemoryStream ms = new MemoryStream();
                profilePic.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                //get Base64
                var SigBase64 = Convert.ToBase64String(byteImage);

                //set the dataIntermed profile pic fields
                dataIntermed.profilePic = SigBase64;
            }
            //catch the custom made Fault Exception which is ArgumentOutOfRangeFault
            catch (FaultException<ArgumentOutOfRangeFault> exception)
            {
                //create an error response
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(exception.Detail.Description);
                //log message to file
                logHelper.log($"[ERROR] Get() - GetValuesForEntry is NOT successfuly called. Index {id} is out of range. Http response exception is thrown.");
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
                logHelper.log("[ERROR] Get() - GetValuesForEntry is NOT successfuly called. Http response exception is thrown.");
                //throw a http response exception
                throw new HttpResponseException(httpResponseMessage);
            }

            //log message to file
            logHelper.log($"[INFO] Get() - GetValuesForEntry is successfuly called. Index {id} has an account number of {dataIntermed.acct} which is {dataIntermed.fname} {dataIntermed.lname}'s.");

            //return the data intermed object
            return dataIntermed;
        }
    }
}