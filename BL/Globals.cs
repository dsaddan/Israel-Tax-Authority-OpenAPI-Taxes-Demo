using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxesDemo.BL
{
    public class AppGlobals
    {
        private static Dictionary<string, string> C = new Dictionary<string, string>();

        public static void Set(string key, string value)
        {
            if (C.ContainsKey(key))
            {
                C.Remove(key);
            }

            C.Add(key, value);
        }

        public static string Get(string key)
        {
            if (!C.ContainsKey(key))
            {
                return null;
            }

            return C[key];
        }
    }

    public class Globals
    {
        public static void Set(string key, string value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static string Get(string key)
        {
            if (HttpContext.Current.Session[key] == null)
            {
                return null;
            }

            return HttpContext.Current.Session[key].ToString();
        }
    }
}