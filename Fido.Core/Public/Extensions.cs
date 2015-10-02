using System;
using System.Collections.Generic;

namespace Fido.Core
{
    public static class Extensions
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Guid ToGuid(this string GuidAsString)
        {
            return Guid.Parse(GuidAsString);
        }

        public static void SmartAdd<T>(this List<T> List, T Item)
        {
            if (List.Contains(Item) == false)
                List.Add(Item);
        }

        public static bool IsNullOrEmpty(this string Param)
        {
            return (string.IsNullOrEmpty(Param));
        }

        public static bool IsNotNullOrEmpty(this string Param)
        {
            return (!string.IsNullOrEmpty(Param));
        }

        public static string Nvl(this string Param)
        {
            return Param == null ? "" : Param;
        }
    }
}
