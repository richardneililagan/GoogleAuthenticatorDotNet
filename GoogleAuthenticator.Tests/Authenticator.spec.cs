using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace GoogleAuthenticator.Tests
{
    [TestClass]
    public class AuthenticatorSpecs
    {
        [TestMethod]
        public void GenerateSecret_UsingDefaultArguments_ShouldGenerateASecretStringOf16Characters()
        {
            Assert.AreEqual(
                Authenticator.GenerateSecret().Length,
                16
                );
        }

        [TestMethod]
        public void GenerateSecret_WithALengthArgument_ShouldGenerateASecretStringOfTheRightLength ()
        {
            var random = new Random(Environment.TickCount);
            var targetLength = random.Next(10, 100);

            Assert.AreEqual(
                Authenticator.GenerateSecret(targetLength).Length,
                targetLength
                );
        }

        [TestMethod]
        public void Authenticate_WithInvalidCredentials_ShouldReturnFalse ()
        {
            var secret = Base32.GenerateRandom(16);
            var hmac = new HmacWrapper(Base32.ToBytes(secret));

            var correctOTP = hmac.CalculateOneTimePassword();

            // :: just to make sure that we don't accidentally hit that
            //    1/1000000 chance that we actually use the correct OTP
            //    in our test haha
            var testOTP = correctOTP == "000000" ? "111111" : "000000";

            Assert.IsFalse(
                Authenticator.Authenticate(secret, testOTP)
                );
        }

        [TestMethod]
        public void Authenticate_WithValidCredentials_ShouldReturnTrue ()
        {
            var secret = Base32.GenerateRandom(16);
            var hmac = new HmacWrapper(Base32.ToBytes(secret));

            var correctOTP = hmac.CalculateOneTimePassword();

            Assert.IsTrue(
                Authenticator.Authenticate(secret, correctOTP)
                );
        }

        [TestMethod]
        public void Authenticate_WithLeeway_ShouldRespectLeeway ()
        {
            var time_buffer = 30;

            var secret = Base32.GenerateRandom(16);
            var hmac = new HmacWrapper(Base32.ToBytes(secret), time_buffer);

            var firstOTP = hmac.CalculateOneTimePassword();
            Thread.Sleep(time_buffer * 1000);

            var currentOTP = firstOTP;
            while (currentOTP == firstOTP)
            {
                currentOTP = hmac.CalculateOneTimePassword();
            }

            Assert.IsTrue(
                Authenticator.Authenticate(secret, currentOTP, time_buffer)
                );

            Assert.IsTrue(
                Authenticator.Authenticate(secret, firstOTP, time_buffer)
                );
        }
    }
}
