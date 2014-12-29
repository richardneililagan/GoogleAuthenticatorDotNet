/**
 *   GoogleAuthenticator class library
 *   -----------------------------------------------------------------------
 *   
 *   :: A library for generating and authenticating 2FA codes
 *      according to RFC6238 and Google Authenticator app implementation.
 *      http://tools.ietf.org/html/rfc6238
 *      https://github.com/google/google-authenticator
 *      
 *   @author Richard Neil Ilagan [me@richardneililagan.com]
 *   
 *   This software is protected by the GPL v3 license
 *   as specified in [LICENSE.txt].
 */

using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GoogleAuthenticator.Tests")]

namespace GoogleAuthenticator
{
    public static class Authenticator
    {
        // :: how long is each OTP valid for (in seconds).
        //    Google uses 30 seconds in their app.
        private const int TIME_BUFFER_LENGTH = 30;

        /// <summary>
        /// Checks if the given OTP is the expected OTP,
        /// provided the corresponding secret.
        /// </summary>
        /// <param name="secret">The secret ID used to generate OTPs.</param>
        /// <param name="otp">The OTP to check.</param>
        /// <param name="leeway">
        /// How many seconds +/- to allow for checking generated OTPs. 
        /// Useful in high ping situations.
        /// </param>
        /// <returns></returns>
        public static bool Authenticate(string secret, string otp, int leeway = 0)
        {
            var hmac = new HmacWrapper(Base32.ToBytes(secret), TIME_BUFFER_LENGTH);
            leeway = Math.Abs(leeway);

            var result = false;

            while (!result && leeway > 0)
            {
                result |= (otp == hmac.CalculateOneTimePassword(-leeway));
                result |= (otp == hmac.CalculateOneTimePassword(leeway));

                leeway -= TIME_BUFFER_LENGTH;
            }

            return (result |= (otp == hmac.CalculateOneTimePassword()));            
        }

        /// <summary>
        /// Utility function to generate a new Secret ID.
        /// Secret IDs are base 32, which means that there are only several allowable characters
        /// for their string values. This function makes sure that the generated string
        /// is a valid base 32 string.
        /// </summary>
        /// <returns></returns>
        public static string GenerateSecret(int length = 16)
        {
            return Base32.GenerateRandom(length);
        }
    }
}
