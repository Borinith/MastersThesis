using System;
using System.Linq;

namespace ResearchWork.IO.Input
{
    public static class InitialData
    {
        private static Lazy<double[]>? _b;

        private static void Init(int numberOfLevels)
        {
            _b = new Lazy<double[]>(() =>
            {
                var b = Enumerable.Repeat(0d, numberOfLevels + 1).ToArray();
                b[numberOfLevels] = 1;

                return b;
            });
        }

        public static double[] GetB(int numberOfLevels)
        {
            if (_b == null || numberOfLevels != _b.Value.Length + 1)
            {
                Init(numberOfLevels);
            }

            return _b!.Value;
        }
    }
}