using System;

namespace ResearchWork3.Input
{
    public static class InputCommonParameters
    {
        public const double Hbar = 6.62607015E-27 / (2 * Math.PI); // erg*s
        public const double Kb = 1.380649E-16; // erg/K
        public const long C = 29979245800; // cm/s

        // ReSharper disable once InconsistentNaming
        public const double iii = Hbar * Hbar / (2 * 85.4 * Kb);

        public const double Alpha = 0.083;
        public const int MaxCoLevel = 14;

        public static double G(int j)
        {
            return 2 * j + 1;
        }
    }
}