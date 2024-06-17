using DBInterface;
using System;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    /**
     * ValuesController is a ASP.NET Web API controller class
     * It has a Get method that retrieves the total item of the data by calling GetNumEntries method.
     */
    public class ValuesController : ApiController
    {
        //private fields of LogHelper object to help log message to file
        LogHelper logHelper = new LogHelper();

        /**
        * This rest service retrieves the total number of entries in the database.
        * If the http response is a success,  it returns the total number of entries in the database in integer.
        * Otherwise, a http response exception is thrown.
        */
        public int Get()
        {
            //initialize the result
            int res = 0;

            try
            {
                //create a new DataModel object
                DataModel dataModel = new DataModel();
                //get the total number of items in the data by calling GetNumEntries
                res = dataModel.GetNumEntries();

            }
            //catch exception and throw a http response exception
            catch (Exception exception)
            {
                //create an error response
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(exception.Message);
                //log message to file
                logHelper.log($"[ERROR] Get() - GetNumEntries is NOT successfuly called. Http response exception is thrown.");
                //throw a http response exception
                throw new HttpResponseException(httpResponseMessage);
            }

            //log message to file
            logHelper.log($"[INFO] Get() - GetNumEntries is successfuly called. Database entry contains {res} items.");

            //return the result
            return res;
        }
    }
}