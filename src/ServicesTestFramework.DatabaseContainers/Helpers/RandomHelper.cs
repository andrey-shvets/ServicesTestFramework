using System;

namespace ServicesTestFramework.DatabaseContainers.Helpers
{
    internal static class RandomHelper
    {
        private static Random RandomGenerator { get; } = new Random((int)DateTimeOffset.Now.Ticks);

        public static int RandomNumber(int minValue, int maxValue)
        {
            return RandomGenerator.Next(minValue, maxValue);
        }

        public static int RandomPort(int minValue)
        {
            var maxValue = short.MaxValue;
            return RandomNumber(minValue, maxValue);
        }
    }
}
