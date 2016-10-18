using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace SierraAPI.Errorlogger
{
    public class ApiError
    {
        public static string Application { get; set; }
        public static string OptionalNote { get; set; }
        public static bool Production { get; set; }
        private static string dbConnectionBeta = "";
        private static string dbConnectionProd = "";

        static ApiError()
        {
            Application = "unknown";
        }

        internal static void Log(HttpResponseMessage response)
        {
            string errorText;
            errorText = response.ToString() + " : " + response.Content.ReadAsStringAsync().Result;
            InsertErrorToDB(errorText);
        }

        internal static void Log(Exception ex)
        {
            if (ex is WebException)
            {
                LogWebException((WebException)ex);
            }
            else if (ex is AggregateException)
            {
                LogAggregateException((AggregateException)ex);
            }
            else
            {
                InsertErrorToDB(ex.Message + " Inner Exception: " + ex.InnerException);
            }
        }

        internal static void Log(WebException wex)
        {
            LogWebException(wex);
        }

        internal static void Log(AggregateException aex)
        {
            LogAggregateException((AggregateException)aex);
        }

        private static void LogWebException(WebException wex)
        {
            if (wex.Response != null)
            {
                string error;

                using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                {
                    using (StreamReader errorReader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        error = errorReader.ReadToEnd();
                    }
                }
            }
            InsertErrorToDB(wex.Message);
        }

        private static void LogAggregateException(AggregateException aex)
        {
            string error = "";
            int errorCount = 1;
            foreach (Exception ex in aex.InnerExceptions)
            {
                error += "Error #" + errorCount + ": " + ex.Message;
                errorCount++;
            }
            InsertErrorToDB(error);
        }

        internal static void InsertErrorToDB(string message)
        {
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SierraApi_AddError", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.Parameters.AddWithValue("@Application", Application);
                    cmd.Parameters.AddWithValue("@OptionalNote", OptionalNote);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        internal static string getConnectionString()
        {
            if (Production)
            {
                return dbConnectionProd;
            }
            else
            {
                return dbConnectionBeta;
            }
        }
    }
}
