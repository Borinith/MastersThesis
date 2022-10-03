using System;
using System.Linq;

namespace ResearchWork3.Input
{
    public class InitialData
    {
        private static Lazy<double[]> _b;

        public InitialData(int numberOfLevels)
        {
            Init(numberOfLevels);
        }

        private static void Init(int numberOfLevels)
        {
            _b = new Lazy<double[]>(() =>
            {
                var b = Enumerable.Repeat(0d, numberOfLevels + 1).ToArray();
                b[numberOfLevels] = 1;

                return b;
            });
        }

        public static double[] GetB()
        {
            return _b.Value;
        }
    }
}