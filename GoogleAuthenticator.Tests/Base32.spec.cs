using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace GoogleAuthenticator.Tests
{
    using GoogleAuthenticator;
   

    [TestClass]
    public class Base32Specs
    {
        [TestMethod]
        public void GenerateRandom_GivenTargetLength_ShouldReturnStringOfRightLength()
        {
            var targetLength = 16;
            var randomString = Base32.GenerateRandom(targetLength);

            Debug.WriteLine("Generated String : " + randomString);
            Assert.AreEqual(randomString.Length, targetLength);
        }

        [TestMethod]
        public void GenerateRandom_UsingDefaultArguments_ShouldReturnStringOfLengthOne()
        {
            var randomString = Base32.GenerateRandom();

            Debug.WriteLine("Generated String : " + randomString);
            Assert.AreEqual(randomString.Length, 1);
        }

        [TestMethod]
        public void ToBytes_and_ToString_ShouldBeIdempotent()
        {
            var secret = Base32.GenerateRandom(16);
            Debug.WriteLine("Secret to use : " + secret);

            var segueway = Base32.ToBytes(secret);            

            Assert.IsTrue(segueway.Length > 0);

            Assert.AreEqual(
                secret,
                Base32.ToString(segueway)
                );
        }
    }
}
