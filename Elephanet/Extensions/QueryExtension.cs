using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Elephanet.Expressions;
using Npgsql;

namespace Elephanet
{

    public static class StringExtension
    {
        public static string ReplaceDotWithUnderscore(this string text)
        {
            text = text.Replace(".", "_");
            return text;
        }
    }
}
