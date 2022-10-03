using System;

namespace ResearchWork3.Input
{
    public static class InputCommonParameters
    {
        public const double HBAR = 6.62607015E-27 / (2 * Math.PI); // erg*s
        public const double KB = 1.380649E-16; // erg/K
        public const long C = 29979245800; // cm/s

        public const double III = HBAR * HBAR / (2 * 85.4 * KB);

        public const double ALPHA = 0.083;
        public const int MAX_CO_LEVEL = 14;

        public static double G(int j)
        {
            return 2 * j + 1;
        }
    }
}