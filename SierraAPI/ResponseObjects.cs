using System;
using System.Collections.Generic;
using System.Text;

namespace SierraAPI.ResponseObjects
{
        public class AuthResponse
        {
            public string token_type { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        public class Hold
        {
            public string recordType { get; set; }
            public int recordNumber { get; set; }
            public string pickupLocation { get; set; }
            // // neededBy and numberOfCopies are optional and we dont use them currently
            //    public string neededBy { get; set; }
            //    public string numberOfCopies { get; set; }
        }

        public class Patron
        {
            public int id { get; set; }
            public string expirationDate { get; set; }
            public DateTime birthDate { get; set; }
            public int patronType { get; set; }
            public Patroncodes patronCodes { get; set; }
            public string homeLibraryCode { get; set; }
            public Message message { get; set; }
            public Blockinfo blockInfo { get; set; }
            public double moneyOwed { get; set; }
        }

        public class Patroncodes
        {
            public string pcode1 { get; set; }
            public string pcode2 { get; set; }
            public int pcode3 { get; set; }
            public int pcode4 { get; set; }
        }

        public class Message
        {
            public string code { get; set; }
        }

        public class Blockinfo
        {
            public string code { get; set; }
        }

        public class Checkouts
        {
            public int total { get; set; }
            public CheckoutEntry[] entries { get; set; }
        }

        public class CheckoutEntry
        {
            public string id { get; set; }
            public string patron { get; set; }
            public string item { get; set; }
            public string dueDate { get; set; }
            public int numberOfRenewals { get; set; }
            public DateTime outDate { get; set; }
        }

        public class Items
        {
            public int total { get; set; }
            public ItemEntry[] entries { get; set; }
        }

        public class ItemEntry
        {
            public string id { get; set; }
            public DateTime updatedDate { get; set; }
            public DateTime createdDate { get; set; }
            public bool deleted { get; set; }
            public string[] bibIds { get; set; }
            public Location location { get; set; }
            public Status status { get; set; }
            public string barcode { get; set; }
            public string callNumber { get; set; }
        }

        public class Location
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Status
        {
            public string code { get; set; }
            public string display { get; set; }
            public DateTime duedate { get; set; }
        }

        public class Bibs
        {
            public int total { get; set; }
            public BibEntry[] entries { get; set; }
        }

        public class BibEntry
        {
            public string id { get; set; }
            public DateTime updatedDate { get; set; }
            public DateTime createdDate { get; set; }
            public bool deleted { get; set; }
            public bool suppressed { get; set; }
            public Lang lang { get; set; }
            public string title { get; set; }
            public string author { get; set; }
            public Materialtype materialType { get; set; }
            public Biblevel bibLevel { get; set; }
            public int publishYear { get; set; }
            public string catalogDate { get; set; }
            public Country country { get; set; }
        }

        public class Lang
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Materialtype
        {
            public string code { get; set; }
            public string value { get; set; }
        }

        public class Biblevel
        {
            public string code { get; set; }
            public string value { get; set; }
        }

        public class Country
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Fines
        {
            public int total { get; set; }
            public FineEntry[] entries { get; set; }
        }

        public class FineEntry
        {
            public string id { get; set; }
            public string patron { get; set; }
            public string assessedDate { get; set; }
            public string description { get; set; }
            public Chargetype chargeType { get; set; }
            public float itemCharge { get; set; }
            public double processingFee { get; set; }
            public double billingFee { get; set; }
            public double paidAmount { get; set; }
            public Location location { get; set; }
            public string item { get; set; }
        }

        public class Chargetype
        {
            public string code { get; set; }
            public string display { get; set; }
        }

    }
