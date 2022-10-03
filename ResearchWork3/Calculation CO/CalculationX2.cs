using MathNet.Numerics.Interpolation;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.WindowsAPICodePack.Taskbar;
using ResearchWork3.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ResearchWork3.Calculation_CO
{
    public static class CalculationX2
    {
        //-------------------------------------------Calculation X2--------------------------------------------

        public static Task<List<CalculateX2>> CalculationX2Table(InputParametersOfSystem inputParameters,
            CancellationToken cancellationToken, IProgress<double> progress, IProgress<TimeSpan> timeProgress)
        {
            _ = new InitialData(inputParameters.NumberOfLevels);

            var task = Task.Run(() =>
            {
                var lenChi2Table = (int)Math.Round(
                    (inputParameters.NMax -
                     inputParameters.NMin +
                     inputParameters.NStep) /
                    inputParameters.NStep *
                    ((inputParameters.TemperatureKinMax -
                      inputParameters.TemperatureKinMin +
                      inputParameters.TemperatureKinStep) /
                     inputParameters.TemperatureKinStep) *
                    ((inputParameters.TemperatureCmbMax -
                      inputParameters.TemperatureCmbMin +
                      inputParameters.TemperatureCmbStep) /
                     inputParameters.TemperatureCmbStep) *
                    ((inputParameters.N0Max -
                      inputParameters.N0Min +
                      inputParameters.N0Step) /
                     inputParameters.N0Step), 0);

                var chi2TableList = new List<CalculateX2>(lenChi2Table);

                var chNumberOfX2 = 0; //Number of X2

                var rotationLevelsPrG = CalculationRotationLevelsPr(inputParameters.RotationLevelsPr);

                var sw = new Stopwatch();
                sw.Start();

                for (var n = inputParameters.NMin; n <= inputParameters.NMax; n += inputParameters.NStep)
                {
                    for (var temperatureKin = inputParameters.TemperatureKinMin; temperatureKin <= inputParameters.TemperatureKinMax; temperatureKin += inputParameters.TemperatureKinStep)
                    {
                        var alphaParaCoeff = CalculationAlphaPara.Instance.GetAlphaPara3().Interpolate(Convert.ToDouble(temperatureKin));

                        for (var temperatureCmb = inputParameters.TemperatureCmbMin; temperatureCmb <= inputParameters.TemperatureCmbMax; temperatureCmb += inputParameters.TemperatureCmbStep)
                        {
                            var kin = temperatureKin;
                            var cmb = temperatureCmb;
                            var coeff = alphaParaCoeff;
                            var n1 = n;

                            Parallel.For(
                                (int)Math.Round(inputParameters.N0Min / inputParameters.N0Step, 0),
                                (int)Math.Round(inputParameters.N0Max / inputParameters.N0Step, 0) +
                                1,
                                (n0Big, state) =>
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        state.Stop();

                                        return;
                                    }

                                    var n0 = Math.Round(n0Big * inputParameters.N0Step, inputParameters.N0Round);

                                    var aTable = Aa(
                                        inputParameters.NumberOfLevels,
                                        InputTablesCo.Instance.GetEnergyTable(),
                                        InputTablesCo.Instance.GetAEinTable(),
                                        CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniH(),
                                        CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniH2Ortho(),
                                        CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniH2Para(),
                                        CalculationCollisionCoefficientsOfCo.Instance.GetCoCoeffMiniHe(),
                                        Convert.ToDouble(n1),
                                        inputParameters.F,
                                        Convert.ToDouble(kin),
                                        Convert.ToDouble(cmb),
                                        coeff);

                                    var aMatrix = Matrix<double>.Build.DenseOfColumnArrays(aTable).Transpose();
                                    var bVector = Vector<double>.Build.Dense(InitialData.GetB());
                                    var xSolution = aMatrix.Solve(bVector);

                                    var rotationLevelsTh = new double[inputParameters.NumberOfLevels];

                                    for (var level = 0; level <= inputParameters.NumberOfLevels - 1; level++)
                                    {
                                        rotationLevelsTh[level] =
                                            Math.Log10(xSolution[level] * Math.Pow(10, Convert.ToDouble(n0)) / xSolution[0]) -
                                            Math.Log10(InputCommonParameters.G(level));
                                    }

                                    var dTemperatureKin = kin > inputParameters.TemperatureKinPr
                                        ? inputParameters.DTemperatureKinMax
                                        : inputParameters.DTemperatureKinMin;

                                    var dn = n1 > inputParameters.NPr
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

                                    chi2 += Math.Pow(Convert.ToDouble((kin - inputParameters.TemperatureKinPr) / dTemperatureKin), 2) +
                                            Math.Pow(Convert.ToDouble((n1 - inputParameters.NPr) / dn), 2);

                                    var chi2TableRow = new CalculateX2
                                    {
                                        N1 = n1,
                                        Tkin = kin,
                                        N0 = n0,
                                        Tcmb = cmb,
                                        X2 = chi2
                                    };

                                    lock (chi2TableList)
                                    {
                                        chi2TableList.Add(chi2TableRow);
                                    }
                                });

                            if (cancellationToken.IsCancellationRequested)
                            {
                                return null;
                            }

                            chNumberOfX2 += (int)((inputParameters.N0Max -
                                                   inputParameters.N0Min +
                                                   inputParameters.N0Step) /
                                                  inputParameters.N0Step);

                            progress.Report((double)chNumberOfX2 / lenChi2Table * 100);

                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                            TaskbarManager.Instance.SetProgressValue(chNumberOfX2, lenChi2Table);

                            timeProgress.Report(sw.GetEta(chNumberOfX2, lenChi2Table));
                        }
                    }
                }

                //------------------------------------Modification X2 table------------------------------------

                var sortedChi2Table = chi2TableList.OrderBy(x => x.X2).ToList();

                return sortedChi2Table.Where(x => x.X2 <= sortedChi2Table.FirstOrDefault()?.X2 + 1).ToList();
            }, cancellationToken);

            return task;
        }

        private static double[] CalculationRotationLevelsPr(IReadOnlyList<double> rotationLevelsPr)
        {
            var rotationLevelsPrG = new double[rotationLevelsPr.Count];

            for (var i = 0; i < rotationLevelsPrG.Length; i++)
            {
                rotationLevelsPrG[i] = rotationLevelsPr[i] - Math.Log10(InputCommonParameters.G(i));
            }

            return rotationLevelsPrG;
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
            IReadOnlyList<CubicSpline[]> coCoeffminiH,
            IReadOnlyList<CubicSpline[]> coCoeffminiH2Ortho,
            IReadOnlyList<CubicSpline[]> coCoeffminiH2Para,
            IReadOnlyList<CubicSpline[]> coCoeffminiHe,
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
                    (alphaparaCoeff * coCoeffminiH2Para[i][j].Interpolate(tkin) +
                     (1 - alphaparaCoeff) * coCoeffminiH2Ortho[i][j].Interpolate(tkin)) +
                    (1 - f) * coCoeffminiH[i][j].Interpolate(tkin) +
                    InputCommonParameters.ALPHA * coCoeffminiHe[i][j].Interpolate(tkin)) *
                   (n / (1 + InputCommonParameters.ALPHA - f / 2)) +
                   Cocmb(i, j, entab, aEintab, tcmb);
        }

        private static double[][] Aa(
            int ur,
            IReadOnlyList<double> entab,
            IReadOnlyList<double> aEintab,
            IReadOnlyList<CubicSpline[]> coCoeffminiH,
            IReadOnlyList<CubicSpline[]> coCoeffminiH2Ortho,
            IReadOnlyList<CubicSpline[]> coCoeffminiH2Para,
            IReadOnlyList<CubicSpline[]> coCoeffminiHe,
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

    public class CalculateX2
    {
        public decimal N1 { get; set; }

        public decimal Tkin { get; set; }

        public decimal N0 { get; set; }

        public decimal Tcmb { get; set; }

        public double X2 { get; set; }
    }
}