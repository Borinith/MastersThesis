using System.Linq;

namespace ResearchWork.IO.Input
{
    public static class InitialData
    {
        private static readonly object Padlock = new();
        private static double[]? _b;

        private static double[] Init(int numberOfLevels)
        {
            var b = Enumerable.Repeat(0d, numberOfLevels + 1).ToArray();
            b[numberOfLevels] = 1;

            return b;
        }

        public static double[] GetB(int numberOfLevels)
        {
            lock (Padlock)
            {
                if (_b == null || _b.Length != numberOfLevels + 1)
                {
                    _b = Init(numberOfLevels);
                }

                return _b;
            }
        }
    }
}