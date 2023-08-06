using MathNet.Numerics.Interpolation;
using MathNet.Numerics.LinearAlgebra;
using ResearchWork.IO.Input;
using ResearchWork.IO.Models;
using System;
using System.Collections.Generic;

namespace ResearchWork.Calculation.CalculationCO
{
    public static class CalculationX2
    {
        public static CalculateX2 CalculateX2(
            InputParametersOfSystem inputParameters,
            decimal n0,
            decimal nCopy,
            decimal kinCopy,
            decimal cmbCopy,
            double coeffCopy,
            IReadOnlyList<double> rotationLevelsPrG)
        {
            var aTable = Aa(
                inputParameters.NumberOfLevels,
                InputTablesCo.Instance.GetEnergyTable(),
                InputTablesCo.Instance.GetAEinTable(),
                CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniH(),
                CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniH2Ortho(),
                CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniH2Para(),
                CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniHe(),
                Convert.ToDouble(nCopy),
                inputParameters.F,
                Convert.ToDouble(kinCopy),
                Convert.ToDouble(cmbCopy),
                coeffCopy);

            var aMatrix = Matrix<double>.Build.DenseOfColumnArrays(aTable).Transpose();
            var bVector = Vector<double>.Build.Dense(InitialData.GetB(inputParameters.NumberOfLevels));
            var xSolution = aMatrix.Solve(bVector);

            var rotationLevelsTh = new double[inputParameters.NumberOfLevels];

            for (var level = 0; level <= inputParameters.NumberOfLevels - 1; level++)
            {
                rotationLevelsTh[level] =
                    Math.Log10(xSolution[level] * Math.Pow(10, Convert.ToDouble(n0)) / xSolution[0]) -
                    Math.Log10(InputCommonParameters.G(level));
            }

            var dTemperatureKin = kinCopy > inputParameters.TemperatureKinPr
                ? inputParameters.DTemperatureKinMax
                : inputParameters.DTemperatureKinMin;

            var dn = nCopy > inputParameters.NPr
                ? inputParameters.DnMax
                : inputParameters.DnMin;

            double chi2 = 0;

            for (var i = 0;
                 i <
                 Math.Min(inputParameters.DRotationLevelsPr.GetUpperBound(0) + 1,
                     inputParameters.NumberOfLevels);
                 i++)
            {
                chi2 += Math.Pow(
                    (rotationLevelsTh[i] - rotationLevelsPrG[i]) /
                    inputParameters.DRotationLevelsPr[i], 2);
            }

            chi2 += Math.Pow(Convert.ToDouble((kinCopy - inputParameters.TemperatureKinPr) / dTemperatureKin), 2) +
                    Math.Pow(Convert.ToDouble((nCopy - inputParameters.NPr) / dn), 2);

            return new CalculateX2
            {
                N1 = nCopy,
                Tkin = kinCopy,
                N0 = n0,
                Tcmb = cmbCopy,
                X2 = chi2
            };
        }

        private static double AEin(int ii, int jj, IReadOnlyList<double> aEintab)
        {
            if (ii == jj + 1)
            {
                return aEintab[ii - 1];
            }

            return 0;
        }

        private static double Cocmb(int ii, int jj, IReadOnlyList<double> entab, IReadOnlyList<double> aEintab, double tcmb)
        {
            if (ii == jj + 1)
            {
                return AEin(ii, jj, aEintab) *
                       (1 +
                        1 /
                        (Math.Pow(Math.E,
                             2 *
                             Math.PI *
                             InputCommonParameters.HBAR *
                             InputCommonParameters.C *
                             (entab[ii] - entab[jj]) /
                             (InputCommonParameters.KB * tcmb)) -
                         1));
            }

            if (jj == ii + 1)
            {
                return InputCommonParameters.G(jj) /
                       InputCommonParameters.G(ii) *
                       AEin(jj, ii, aEintab) *
                       (1 /
                        (Math.Pow(Math.E,
                             2 *
                             Math.PI *
                             InputCommonParameters.HBAR *
                             InputCommonParameters.C *
                             (entab[jj] - entab[ii]) /
                             (InputCommonParameters.KB * tcmb)) -
                         1));
            }

            return 0;
        }

        private static double Wtot(
            int i,
            int j,
            IReadOnlyList<double> entab,
            IReadOnlyList<double> aEintab,
            IReadOnlyList<CubicSpline?[]> coCoeffminiH,
            IReadOnlyList<CubicSpline?[]> coCoeffminiH2Ortho,
            IReadOnlyList<CubicSpline?[]> coCoeffminiH2Para,
            IReadOnlyList<CubicSpline?[]> coCoeffminiHe,
            double n,
            double f,
            double tkin,
            double tcmb,
            double alphaparaCoeff)
        {
            if (i == j)
            {
                return 0;
            }

            return (f /
                    2 *
                    (alphaparaCoeff * coCoeffminiH2Para[i][j]!.Interpolate(tkin) +
                     (1 - alphaparaCoeff) * coCoeffminiH2Ortho[i][j]!.Interpolate(tkin)) +
                    (1 - f) * coCoeffminiH[i][j]!.Interpolate(tkin) +
                    InputCommonParameters.ALPHA * coCoeffminiHe[i][j]!.Interpolate(tkin)) *
                   (n / (1 + InputCommonParameters.ALPHA - f / 2)) +
                   Cocmb(i, j, entab, aEintab, tcmb);
        }

        private static double[][] Aa(
            int ur,
            IReadOnlyList<double> entab,
            IReadOnlyList<double> aEintab,
            IReadOnlyList<CubicSpline?[]> coCoeffminiH,
            IReadOnlyList<CubicSpline?[]> coCoeffminiH2Ortho,
            IReadOnlyList<CubicSpline?[]> coCoeffminiH2Para,
            IReadOnlyList<CubicSpline?[]> coCoeffminiHe,
            double n,
            double f,
            double tkin,
            double tcmb,
            double alphaparaCoeff)
        {
            var aaaTab = new double[ur + 1][];

            for (var i = 0; i < ur; i++)
            {
                aaaTab[i] = new double[ur];

                for (var j = 0; j < ur; j++)
                {
                    if (i == j)
                    {
                        double sum = 0;

                        for (var k = 0; k < ur; k++)
                        {
                            sum += Wtot(i, k, entab, aEintab, coCoeffminiH, coCoeffminiH2Ortho, coCoeffminiH2Para,
                                coCoeffminiHe, n, f, tkin, tcmb, alphaparaCoeff);
                        }

                        aaaTab[i][j] = -sum;
                    }
                    else
                    {
                        aaaTab[i][j] = Wtot(j, i, entab, aEintab, coCoeffminiH, coCoeffminiH2Ortho, coCoeffminiH2Para,
                            coCoeffminiHe, n, f, tkin, tcmb, alphaparaCoeff);
                    }
                }
            }

            aaaTab[ur] = new double[ur];

            for (var j = 0; j < ur; j++)
            {
                aaaTab[ur][j] = InputCommonParameters.G(j);
            }

            return aaaTab;
        }
    }
}