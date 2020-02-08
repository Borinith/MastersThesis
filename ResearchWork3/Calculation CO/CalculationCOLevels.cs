using ResearchWork3.Input;

namespace ResearchWork3.Calculation_CO
{
    internal class CalculationCoLevels
    {
        //----------------------------------------Calculation CO levels----------------------------------------

        public readonly double[][] TabCoCoeff;

        public CalculationCoLevels()
        {
            var chCoCoeff = 0;

            TabCoCoeff = new double[InputCommonParameters.MaxCoLevel * (InputCommonParameters.MaxCoLevel + 1) / 2][];

            for (var i = 1; i <= InputCommonParameters.MaxCoLevel; i++)
            {
                for (var j = i - 1; j >= 0; j--)
                {
                    TabCoCoeff[i + chCoCoeff - 1] = new double[2];

                    TabCoCoeff[i + chCoCoeff - 1][0] = i;
                    TabCoCoeff[i + chCoCoeff - 1][1] = j;
                    chCoCoeff++;
                }

                chCoCoeff--;
            }
        }
    }
}