using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using System.Web.Script.Serialization;
using System.Net;
using System.IO;

using System.Reflection;
using System.Data;
using SierraAPI.ResponseObjects;
using SierraAPI.Errorlogger;

namespace Sierra
{
    public class Api
    {
        private string _uri, _key, _secret;
        private string _token;
        private DateTime _token_expires;
        public bool isInitialized;
        private static HttpClient Client = new HttpClient();

        /// <summary>
        /// initialize the API
        /// </summary>
        /// <param name="uri">Base uri to API endpoint</param>
        /// <param name="key">Key value </param>
        /// <param name="secret">Secret value</param>
        public Api(string uri, string key, string secret, bool isProduction = false)
        {
            _uri = uri;
            _key = key;
            _secret = secret;
            ApiError.Application = Assembly.GetCallingAssembly().FullName;
            ApiError.Production = isProduction;
        }

        public Patron GetPatronByBarcode(string barcode)
        {
            string uri = String.Format("patrons/find?barcode={0}", barcode);
            var patron = GetAsync<Patron>(uri).Result;

            return patron;
        }

        public Checkouts GetCheckoutsByPatronId(int patronid)
        {
            string uri = String.Format("patrons/{0}/checkouts", patronid);
            var checkouts = GetAsync<Checkouts>(uri).Result;

            return checkouts;
        }

        public Items GetItemsById(string ids)
        {
            string uri = String.Format("items?id={0}", ids);
            var items = GetAsync<Items>(uri).Result;

            return items;
        }
        public Bibs GetBibsById(string ids)
        {
            string uri = String.Format("bibs?id={0}", ids);
            var bibs = GetAsync<Bibs>(uri).Result;

            return bibs;
        }

        public Fines GetFinesByPatronId(int id)
        {
            string uri = String.Format("patrons/{0}/fines", id);
            var fines = GetAsync<Fines>(uri).Result;

            return fines;
        }
        
        public Boolean PlaceHold(int patronID, string recordType, int recordNumber, string pickupLocation, int responseID=0)
        {
            bool result = false;
            if (responseID != 0) { ApiError.OptionalNote = responseID.ToString(); }
            string uri = String.Format("patrons/{0}/holds/requests", patronID);

            var hold = new Hold { recordType = recordType, recordNumber = recordNumber, pickupLocation = pickupLocation };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(hold);

            HttpResponseMessage response = PostAsync(uri, json).Result;

            if (response.Content.ReadAsStringAsync().Result.Contains("already on hold")){//500 error but its OK hold already placed
                result = true;
            }
            else if (!response.IsSuccessStatusCode) {
                result = false;
            }
            else
            {
                result = true;
            }

            ApiError.OptionalNote = null; //reset
            return result;
        }
        
        public Boolean PinTest(string uribase, string barcode, int pin){

            string uri = uribase + String.Format("{0}/{1}/pintest", barcode, pin);
            var response = GetStringAsync(uri).Result;
            return response.Contains("RETCOD=0");
        }

        public async Task<T> GetAsync<T>(string uri)
        {
           var serializer = new JavaScriptSerializer();
           var response = new HttpResponseMessage();
           string content = "";

           try
           {
               CheckToken();
               response = await Client.GetAsync(uri);

               if (!response.IsSuccessStatusCode) {  ApiError.Log(response); }
               content = await response.Content.ReadAsStringAsync();
           }
           catch (WebException wex)
           {
               ApiError.Log(wex);
           }
           catch (AggregateException aex)
           {
               ApiError.Log(aex);
           } 
           catch (Exception ex)
           {
               ApiError.Log(ex);
           }

           var t = serializer.Deserialize<T>(content);
           return t;

        }

        public async Task<String> GetStringAsync(String uri, bool needsAuthToken = false)
        {
            if (needsAuthToken) { CheckToken(); }

            string content="";

            try
            {
                content = await Client.GetStringAsync(uri);
                
            }
            catch (WebException wex)
            {
                ApiError.Log(wex);
            }
            catch (AggregateException aex)
            {
                ApiError.Log(aex);
            }
            catch (Exception ex)
            {
                ApiError.Log(ex);
            }

            return content;

        }

        public async Task<HttpResponseMessage> PostAsync(string uri, string json)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                //   throw new WebException("Unable to locate the Server with 'www.contoso.com' Uri.", WebExceptionStatus.NameResolutionFailure);
                CheckToken();               
                response = await Client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode) { ApiError.Log(response); }
            }
            catch (WebException wex)
            {
                ApiError.Log(wex);
            }
            catch (AggregateException aex)
            {
                ApiError.Log(aex);
            }
            catch (Exception ex)
            {
                ApiError.Log(ex);
            }

            return response;
            
        }

        private async Task<bool> GetBearerTokenAsync()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_uri);

            var byteArray = new UTF8Encoding().GetBytes(_key + ":" + _secret);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            var request = new HttpRequestMessage(HttpMethod.Post, "/iii/sierra-api/v2/token");
            request.Content = new FormUrlEncodedContent(formData);
            
            try
            {
               //throw new WebException("token error bruh' Uri.", WebExceptionStatus.NameResolutionFailure);
                var response = await client.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ApiError.Log(response);
                    return false;
                }

                var serializer = new JavaScriptSerializer();
                AuthResponse SierraResponse = serializer.Deserialize<AuthResponse>(payload);

                _token_expires = DateTime.UtcNow.AddSeconds(SierraResponse.expires_in);
                _token = SierraResponse.access_token;
            }
            catch (Exception ex)
            {
                _token = null;
               ApiError.Log(ex);
            }
            finally
            {
                client.Dispose();
            }

            // no token no init
            return _token != null;
        }

        private void InitializeHttpClient()
        {
            Client.BaseAddress = new Uri(_uri);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
        }

        public bool CheckToken()
        {
            if (DateTime.UtcNow >= _token_expires)
            {
                isInitialized = GetBearerTokenAsync().GetAwaiter().GetResult();
                //use static HttpClient for all subsequent requests
                if (isInitialized) 
                { 
                    InitializeHttpClient();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}