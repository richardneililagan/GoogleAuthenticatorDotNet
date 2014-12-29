using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoogleAuthenticator.Tests
{
    using GoogleAuthenticator;
    using System.Diagnostics;

    [TestClass]
    public class HmacBufferSpecs
    {
        [TestMethod]
        public void CalculateOneTimePassword_UsingDefaultArguments_ShouldGiveSixDigitInteger()
        {
            var secret = "ABCDABCDABCDABCD";
            var hmac = new HmacWrapper(Base32.ToBytes(secret));

            var otp = hmac.CalculateOneTimePassword();

            Debug.WriteLine("Generated OTP : " + otp);

            Assert.AreEqual(otp.Length, 6);            
        }
    }
}
