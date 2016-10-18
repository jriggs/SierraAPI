using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SierraAPI.ResponseObjects;

namespace SierraAPI
{
    public static class Extensions
    {
        public static string GetCommaSeparatedItemIdsFromCheckout(Checkouts checkout)
        {
            List<string> strings = new List<string> { };

            if (checkout.entries.Length == 0) { return ""; }

            for (int i = 0; i < checkout.entries.Length; i++)
            {
                string itemurl = checkout.entries[i].item;
                strings.Add(itemurl.Substring(itemurl.LastIndexOf("/") + 1));
            }

            return string.Join(",", strings);

        }

        public static string GetCommaSeparatedBibIdsFromItemsCheckedOut(Items items)
        {
            List<string> strings = new List<string> { };

            if (items.entries.Length == 0) { return ""; }

            for (int i = 0; i < items.entries.Length; i++)
            {
                string[] bibId = items.entries[i].bibIds;
                strings.AddRange(bibId);
            }

            return string.Join(",", strings);

        }
    }
}
