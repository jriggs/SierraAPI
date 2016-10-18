using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sierra;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using SierraAPI.ResponseObjects;


namespace APITester
{
    class Program
    {
        static void Main(string[] args)
        {

            string uri ="", key ="", secret = ""; 
            bool is_prod = false;
            if ( ConfigurationManager.AppSettings["PRODUCTION"] =="TRUE" ){
                uri = ConfigurationManager.AppSettings["PatronIOHost"];
                key = ConfigurationManager.AppSettings["ClientKey"];
                secret = ConfigurationManager.AppSettings["ClientSecret"];
                is_prod = true;
            } else{
              uri =     ConfigurationManager.AppSettings["PatronIOHostBeta"];
              key =     ConfigurationManager.AppSettings["ClientKey"];
              secret=   ConfigurationManager.AppSettings["ClientSecret"];
            }
            
            //INITIALIZE API CLASS
            Sierra.Api API = new Sierra.Api(uri, key, secret, is_prod);
            

            //OPTIONAL MAKE SURE CONNECTION IS GOOD
            // var initialized = API.CheckToken();


            //GET DETAILS BY ITEM ID
            var item = API.GetItemsById("123456");
            var bib = item.entries.FirstOrDefault();
            var bibdetail = API.GetBibsById(bib.id);
            

            //GET ALL ITEMS CHECKED OUT TO PATRON
            var patron = API.GetPatronByBarcode("123456");
            var patron_checkouts = API.GetCheckoutsByPatronId(patron.id);
            //extract item ids from checkouts object
            var checked_out_ids = SierraAPI.Extensions.GetCommaSeparatedItemIdsFromCheckout(patron_checkouts);
            //get all items
            var items = API.GetItemsById(checked_out_ids);
            //extract bib ids from items object
            var bibs = SierraAPI.Extensions.GetCommaSeparatedBibIdsFromItemsCheckedOut(items);
            //finally get all bib information
            var bib_entries = API.GetBibsById(bibs);


            //PLACE A HOLD
            var patron2 = API.GetPatronByBarcode("123456");
            //returns true if successful or already on hold
            var t = API.PlaceHold(patron2.id, "b", 123456, "1");


            //GET PATRON FINE INFORMATION
            var patron3 = API.GetPatronByBarcode("123456");
            var fines = API.GetFinesByPatronId(patron3.id);

        }
    }
}
