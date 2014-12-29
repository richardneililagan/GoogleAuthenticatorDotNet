using System;
using System.Linq;
using System.Security.Cryptography;

namespace GoogleAuthenticator
{
    /// <summary>
    /// A byte-buffer used by the SHA1-HMAC algorithm implemented by Google Authenticator
    /// and RFC6238.
    /// </summary>
    /// 
    /// http://tools.ietf.org/html/rfc6238
    internal class HmacWrapper
    {
        private readonly int        TIME_BUFFER = 30;
        private readonly HMACSHA1   BUFFER;        

        internal HmacWrapper(byte[] secret, int time_buffer = 30)
        {
            BUFFER = new HMACSHA1(secret);
            TIME_BUFFER = time_buffer;
        }

        /// <summary>
        /// Calculates the six-digit OTP used by Google Authenticator using the current UNIX timestamp.
        /// </summary>        
        internal string CalculateOneTimePassword(int offset = 0, int buffer = 0)
        {
            var referenceTimestamp = Convert.ToInt64(
                UnixTimestamp.GetTimestamp(offset) / (buffer <= 0 ? TIME_BUFFER : buffer)
                );

            var seed = BitConverter.GetBytes(referenceTimestamp).Reverse().ToArray();

            // ::   black magic
            //      more info on the RFC6238 algorithm here:
            //      http://tools.ietf.org/html/rfc6238
            var hash = BUFFER.ComputeHash(seed);
            var delta = hash.Last() & 0x0F;

            var otp = ((
                (hash[delta] & 0x7F) << 24      |
                (hash[delta + 1] & 0xff) << 16  |
                (hash[delta + 2] & 0xff) << 8   |
                (hash[delta + 3] & 0xff)
            ) % 1000000).ToString();

            // :: pad the OTP with zeroes if we have less than 6 digits
            while (otp.Length < 6)
            {
                // TODO refactor this into something more efficient
                otp = "0" + otp;
            }

            return otp;
        }
    }
}
