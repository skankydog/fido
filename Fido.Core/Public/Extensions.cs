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
    }
}
