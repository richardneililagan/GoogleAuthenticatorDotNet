using System;

namespace GoogleAuthenticator
{
    internal class UnixTimestamp
    {
        private static DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0);        

        /// <summary>
        /// Calculates the number of seconds since the epoch using the current date and time,
        /// adjusted using by the specified number of seconds. (GMT)
        /// </summary>
        /// <param name="offset">The number of seconds to adjust the current date and time</param>        
        internal static Int64 GetTimestamp(int offset = 0)
        {
            return GetTimestamp(DateTime.UtcNow, offset);
        }

        /// <summary>
        /// Calculates the number of seconds since the epoch using a reference date and offset. (GMT)
        /// </summary>
        /// <param name="referenceDate">Date to calculate timestamp from</param>
        /// <param name="offset">The number of seconds to adjust the current date and time</param>           
        internal static Int64 GetTimestamp(DateTime referenceDate, int offset)
        {
            var referenceDateUtc = referenceDate.AddSeconds(offset).ToUniversalTime();
            return Convert.ToInt64(
                Math.Round((referenceDateUtc - EPOCH).TotalSeconds)
                );
        }
    }
}
