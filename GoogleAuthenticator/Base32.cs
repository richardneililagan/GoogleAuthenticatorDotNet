using System;
using System.Text;

namespace GoogleAuthenticator
{
    /// <remarks>
    /// Implementation of a Base32 conversion wrapper based off of
    /// http://stackoverflow.com/a/7135008/304588
    /// </remarks>    
    internal static class Base32
    {
        private const int   IN_BYTE_LENGTH  = 8;    // standard ASCII format
        private const int   OUT_BYTE_LENGTH = 5;    // Base32 : 00000
        private const int   DELTA_BYTE_LENGTH = IN_BYTE_LENGTH - OUT_BYTE_LENGTH;
        private const int   BASE_MAX_BYTE_VALUE = 31;

        private static char[] VALID_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        internal static string GenerateRandom(int length = 1)
        {            
            var random = new Random(Environment.TickCount);
            var ret = new StringBuilder();

            for (var i = 0; i < length; i++)
            {                                
                ret.Append(
                    VALID_CHARS[random.Next(VALID_CHARS.Length)]
                    );
            }

            return ret.ToString();
        }

        public static byte[] ToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }

            input = input.TrimEnd('='); //remove padding characters

            int byteCount = input.Length * OUT_BYTE_LENGTH / IN_BYTE_LENGTH; //this must be TRUNCATED
            byte[] returnArray = new byte[byteCount];

            byte curByte = 0, 
                 bitsRemaining = IN_BYTE_LENGTH
                 ;
            int mask = 0, 
                arrayIndex = 0
                ;

            foreach (char c in input)
            {
                int cValue = CharToValue(c);

                if (bitsRemaining > OUT_BYTE_LENGTH)
                {
                    mask = cValue << (bitsRemaining - OUT_BYTE_LENGTH);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= OUT_BYTE_LENGTH;
                }
                else
                {
                    mask = cValue >> (OUT_BYTE_LENGTH - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (DELTA_BYTE_LENGTH + bitsRemaining));
                    bitsRemaining += DELTA_BYTE_LENGTH;
                }
            }

            //if we didn't end with a full byte
            if (arrayIndex != byteCount)
            {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }

        public static string ToString(byte[] input)
        {            
            // :: failsafe
            if (input == null || input.Length == 0)
            {
                throw new ArgumentNullException("input");
            }

            byte nextChar = 0,
                 bitsRemaining = OUT_BYTE_LENGTH
                 ;

            int currentCharCount = 0;
            int charCount = (int)Math.Ceiling(input.Length / 5d) * IN_BYTE_LENGTH;

            var ret = new StringBuilder();
            foreach (byte b in input)
            {
                nextChar = (byte)(nextChar | (b >> (IN_BYTE_LENGTH - bitsRemaining)));
                ret.Append(ValueToChar(nextChar));
                currentCharCount++;

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((b >> (DELTA_BYTE_LENGTH - bitsRemaining)) & BASE_MAX_BYTE_VALUE);
                    ret.Append(ValueToChar(nextChar));
                    bitsRemaining += OUT_BYTE_LENGTH;
                    currentCharCount++;
                }

                bitsRemaining -= DELTA_BYTE_LENGTH;

                // :: better if we don't evaluate this except when we're on the last [input] byte
                nextChar = (byte)((b << bitsRemaining) & BASE_MAX_BYTE_VALUE);
            }

            // :: just in case we didn't end with a full char
            if (currentCharCount != charCount)
            {
                ret.Append(ValueToChar(nextChar));
                while (++currentCharCount != charCount)
                {
                    // pad remaining characters
                    ret.Append('=');
                }
            }

            return ret.ToString();            
        }

        private static int CharToValue(char c)
        {
            int value = (int)c;            

            //65-90 == uppercase letters
            if (value < 91 && value > 64)
            {
                return value - 65;
            }
            //50-55 == numbers 2-7
            if (value < 56 && value > 49)
            {
                return value - 24;
            }
            //97-122 == lowercase letters
            if (value < 123 && value > 96)
            {
                return value - 97;
            }

            throw new ArgumentException("Character is not a Base32 character.", "c");
        }

        private static char ValueToChar(byte b)
        {
            if (b < 26)
            {
                return (char)(b + 65);
            }

            if (b < 32)
            {
                return (char)(b + 24);
            }

            throw new ArgumentException("Byte is not a value Base32 value.", "b");
        }

    }
}
