using System;

namespace ServicesTestFramework.DatabaseContainers.Helpers
{
    internal static class RandomHelper
    {
        private const string AlphanumericChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string AlphabeticChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static Random RandomGenerator { get; } = new Random((int)DateTimeOffset.Now.Ticks);

        public static int RandomNumber(int minValue = 0, int maxValue = short.MaxValue)
        {
            return RandomGenerator.Next(minValue, maxValue);
        }

        /// <summary>
        /// Generates random alphanumeric string of specified length.
        /// </summary>
        public static string RandomString(int length = 7)
        {
            return RandomString(AlphanumericChars, length);
        }

        private static string RandomString(string allowedChars, int length)
        {
            if (length == 0)
                return string.Empty;

            var chars = new char[length];

            for (var i = 0; i < length; i++)
                chars[i] = allowedChars[RandomGenerator.Next(0, allowedChars.Length)];

            return new string(chars);
        }
    }
}
