using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace TaxesDemo.BL
{
    public class Tracer
    {
        public static void WriteLine(string s)
        {
            Trace.WriteLine(s);
        }
    }
}