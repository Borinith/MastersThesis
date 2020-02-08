using MathNet.Numerics.Interpolation;
using ResearchWork3.Input;
using System;

namespace ResearchWork3
{
    internal class CalculationAlphaPara
    {
        //-------------------------------------Calculation AlphaPara of CO-------------------------------------

        public readonly CubicSpline AlphaPara3;

        public CalculationAlphaPara()
        {
            var sizeTemperature = (int)Math.Round((200 - 1 + 0.1) / 0.1, 0); // Temperature from 1 to 200

            var xValuesT = new double[sizeTemperature];
            var yValuesT = new double[sizeTemperature];

            var alphaPara2 = new double[sizeTemperature][];

            for (var i = 0; i < sizeTemperature; i++)
            {
                alphaPara2[i] = new double[2];

                for (var j = 0; j <= 1; j++)
                {
                    if (j == 0)
                    {
                        alphaPara2[i][j] = (double)i / 10 + 1.0;
                    }
                    else
                    {
                        alphaPara2[i][j] = AlphaPara((double)i / 10 + 1.0);
                    }
                }
            }

            for (var i = 0; i < sizeTemperature; i++)
            {
                xValuesT[i] = alphaPara2[i][0];
                yValuesT[i] = alphaPara2[i][1];
            }

            AlphaPara3 = CubicSpline.InterpolateNaturalSorted(xValuesT, yValuesT);
        }

        private static double AlphaPara(double temperature)
        {
            double sum1 = 0;
            double sum2 = 0;

            for (var l = 0; l <= 1000; l += 2)
            {
                var pow1 = Math.Pow(Math.E, -InputCommonParameters.Hbar * InputCommonParameters.Hbar * l * (l + 1) / (2 * InputCommonParameters.iii * InputCommonParameters.Kb * temperature));

                sum1 += (2 * l + 1) * pow1;
            }

            for (var l = 1; l <= 1001; l += 2)
            {
                var pow2 = Math.Pow(Math.E, -InputCommonParameters.Hbar * InputCommonParameters.Hbar * l * (l + 1) / (2 * InputCommonParameters.iii * InputCommonParameters.Kb * temperature));

                sum2 += (2 * l + 1) * pow2;
            }

            var beta = sum1 / (3 * sum2);

            return beta / (1 + beta);
        }
    }
}