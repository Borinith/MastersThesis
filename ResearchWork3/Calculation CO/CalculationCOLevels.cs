using ResearchWork3.Input;
using System;

namespace ResearchWork3.Calculation_CO
{
    internal class CalculationCoLevels
    {
        //----------------------------------------Calculation CO levels----------------------------------------

        private static readonly Lazy<CalculationCoLevels> Lazy = new(() => new CalculationCoLevels());

        private double[][] _tabCoCoeff;

        public CalculationCoLevels()
        {
            Init();
        }

        public static CalculationCoLevels Instance => Lazy.Value;

        private void Init()
        {
            var chCoCoeff = 0;

            _tabCoCoeff = new double[InputCommonParameters.MAX_CO_LEVEL * (InputCommonParameters.MAX_CO_LEVEL + 1) / 2][];

            for (var i = 1; i <= InputCommonParameters.MAX_CO_LEVEL; i++)
            {
                for (var j = i - 1; j >= 0; j--)
                {
                    _tabCoCoeff[i + chCoCoeff - 1] = new double[2];

                    _tabCoCoeff[i + chCoCoeff - 1][0] = i;
                    _tabCoCoeff[i + chCoCoeff - 1][1] = j;
                    chCoCoeff++;
                }

                chCoCoeff--;
            }
        }

        public double[][] GetTabCoCoeff()
        {
            return _tabCoCoeff;
        }
    }
}