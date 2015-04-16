using System;
using System.Linq;
using System.Text;


namespace Elephanet.Helpers
{
    public static class StringHelpers
    {
        public static string EscapeQuotes(this string text)
        {
            return text.Replace("'", "''");
        }
    }
}
