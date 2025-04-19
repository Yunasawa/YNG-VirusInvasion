using System;
using System.Linq;

namespace SupersonicWisdomSDK
{
    public class SwEnumUtils
    {
        public static string[] GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => e.ToString())
                       .ToArray();
        }
    }
}