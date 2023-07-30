using MathNet.Numerics.Interpolation;
using ResearchWork.IO.Input;
using System;

namespace ResearchWork.Calculation
{
    public class CalculationAlphaPara
    {
        private static readonly Lazy<CalculationAlphaPara> Lazy = new(() => new CalculationAlphaPara());

        private CubicSpline _alphaPara3 = null!;

        public CalculationAlphaPara()
        {
            Init();
        }

        public static CalculationAlphaPara Instance => Lazy.Value;

        private void Init()
        {
            var sizeTemperature = Convert.ToInt32((200m - 1m + 0.1m) / 0.1m); // Temperature from 1 to 200

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

            _alphaPara3 = CubicSpline.InterpolateNaturalSorted(xValuesT, yValuesT);
        }

        private static double AlphaPara(double temperature)
        {
            double sum1 = 0;
            double sum2 = 0;

            for (var l = 0; l <= 1000; l += 2)
            {
                var pow1 = Math.Pow(Math.E, -InputCommonParameters.HBAR * InputCommonParameters.HBAR * l * (l + 1) / (2 * InputCommonParameters.III * InputCommonParameters.KB * temperature));

                sum1 += (2 * l + 1) * pow1;
            }

            for (var l = 1; l <= 1001; l += 2)
            {
                var pow2 = Math.Pow(Math.E, -InputCommonParameters.HBAR * InputCommonParameters.HBAR * l * (l + 1) / (2 * InputCommonParameters.III * InputCommonParameters.KB * temperature));

                sum2 += (2 * l + 1) * pow2;
            }

            var beta = sum1 / (3 * sum2);

            return beta / (1 + beta);
        }

        public CubicSpline GetAlphaPara3()
        {
            return _alphaPara3;
        }
    }
}