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
            var task = Task.Run(() =>
            {
                var initialData = new InitialData(inputParameters);

                var alphaPara = new CalculationAlphaPara();
                var calculationCollisionCoefficientsOfCo = new CalculationCollisionCoefficientsOfCo();

                var lenChi2Table = (int)Math.Round(
                    (initialData.InputParameters.NMax -
                     initialData.InputParameters.NMin +
                     initialData.InputParameters.NStep) /
                    initialData.InputParameters.NStep *
                    ((initialData.InputParameters.TemperatureKinMax -
                      initialData.InputParameters.TemperatureKinMin +
                      initialData.InputParameters.TemperatureKinStep) /
                     initialData.InputParameters.TemperatureKinStep) *
                    ((initialData.InputParameters.TemperatureCmbMax -
                      initialData.InputParameters.TemperatureCmbMin +
                      initialData.InputParameters.TemperatureCmbStep) /
                     initialData.InputParameters.TemperatureCmbStep) *
                    ((initialData.InputParameters.N0Max -
                      initialData.InputParameters.N0Min +
                      initialData.InputParameters.N0Step) /
                     initialData.InputParameters.N0Step), 0);

                var chi2TableList = new List<CalculateX2>(lenChi2Table);

                var chNumberOfX2 = 0; //Number of X2

                var rotationLevelsPrG = CalculationRotationLevelsPr(initialData.InputParameters.RotationLevelsPr);

                var sw = new Stopwatch();
                sw.Start();

                for (var n = initialData.InputParameters.NMin;
                     n <= initialData.InputParameters.NMax;
                     n = Math.Round(n + initialData.InputParameters.NStep, initialData.InputParameters.NRound))
                {
                    for (var temperatureKin = initialData.InputParameters.TemperatureKinMin;
                         temperatureKin <= initialData.InputParameters.TemperatureKinMax;
                         temperatureKin = Math.Round(temperatureKin + initialData.InputParameters.TemperatureKinStep,
                             initialData.InputParameters.TemperatureKinRound))
                    {
                        var alphaParaCoeff = alphaPara.AlphaPara3.Interpolate(temperatureKin);

                        for (var temperatureCmb = initialData.InputParameters.TemperatureCmbMin;
                             temperatureCmb <= initialData.InputParameters.TemperatureCmbMax;
                             temperatureCmb = Math.Round(temperatureCmb + initialData.InputParameters.TemperatureCmbStep,
                                 initialData.InputParameters.TemperatureCmbRound))
                        {
                            var kin = temperatureKin;
                            var cmb = temperatureCmb;
                            var coeff = alphaParaCoeff;
                            var n1 = n;

                            Parallel.For(
                                (int)Math.Round(initialData.InputParameters.N0Min / initialData.InputParameters.N0Step, 0),
                                (int)Math.Round(initialData.InputParameters.N0Max / initialData.InputParameters.N0Step, 0) +
                                1,
                                (n0Big, state) =>
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        state.Stop();

                                        return;
                                    }

                                    var n0 = Math.Round(n0Big * initialData.InputParameters.N0Step,
                                        initialData.InputParameters.N0Round);

                                    var aTable = Aa(
                                        initialData.InputParameters.NumberOfLevels,
                                        InputTablesCo.GetEnergyTable(),
                                        InputTablesCo.GetAEinTable(),
                                        calculationCollisionCoefficientsOfCo.CoCoeffminiH,
                                        calculationCollisionCoefficientsOfCo.CoCoeffminiH2Ortho,
                                        calculationCollisionCoefficientsOfCo.CoCoeffminiH2Para,
                                        calculationCollisionCoefficientsOfCo.CoCoeffminiHe,
                                        n1,
                                        initialData.InputParameters.F,
                                        kin,
                                        cmb,
                                        coeff);

                                    var aMatrix = Matrix<double>.Build.DenseOfColumnArrays(aTable).Transpose();
                                    var bVector = Vector<double>.Build.Dense(initialData.GetBb());
                                    var xSolution = aMatrix.Solve(bVector);

                                    var rotationLevelsTh = new double[initialData.InputParameters.NumberOfLevels];

                                    for (var level = 0; level <= initialData.InputParameters.NumberOfLevels - 1; level++)
                                    {
                                        rotationLevelsTh[level] =
                                            Math.Log10(xSolution[level] * Math.Pow(10, n0) / xSolution[0]) -
                                            Math.Log10(InputCommonParameters.G(level));
                                    }

                                    var dTemperatureKin = kin > initialData.InputParameters.TemperatureKinPr
                                        ? initialData.InputParameters.DTemperatureKinMax
                                        : initialData.InputParameters.DTemperatureKinMin;

                                    var dn = n1 > initialData.InputParameters.NPr
                                        ? initialData.InputParameters.DnMax
                                        : initialData.InputParameters.DnMin;

                                    double chi2 = 0;

                                    for (var i = 0;
                                         i <
                                         Math.Min(initialData.InputParameters.DRotationLevelsPr.GetUpperBound(0) + 1,
                                             initialData.InputParameters.NumberOfLevels);
                                         i++)
                                    {
                                        chi2 += Math.Pow(
                                            (rotationLevelsTh[i] - rotationLevelsPrG[i]) /
                                            initialData.InputParameters.DRotationLevelsPr[i], 2);
                                    }

                                    chi2 += Math.Pow((kin - initialData.InputParameters.TemperatureKinPr) / dTemperatureKin, 2) +
                                            Math.Pow((n1 - initialData.InputParameters.NPr) / dn, 2);

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

                            chNumberOfX2 += (int)((initialData.InputParameters.N0Max -
                                                   initialData.InputParameters.N0Min +
                                                   initialData.InputParameters.N0Step) /
                                                  initialData.InputParameters.N0Step);

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
                             InputCommonParameters.Hbar *
                             InputCommonParameters.C *
                             (entab[ii] - entab[jj]) /
                             (InputCommonParameters.Kb * tcmb)) -
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
                             InputCommonParameters.Hbar *
                             InputCommonParameters.C *
                             (entab[jj] - entab[ii]) /
                             (InputCommonParameters.Kb * tcmb)) -
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
                    InputCommonParameters.Alpha * coCoeffminiHe[i][j].Interpolate(tkin)) *
                   (n / (1 + InputCommonParameters.Alpha - f / 2)) +
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

        /*private static double[][] DeleteRow(IReadOnlyList<double[]> table, int strings, int columns, int num)
        {
            var temp = new double[strings][];

            int i;
            var index = 0;

            for (i = 0; i < strings; i++)
            {
                if (i == num)
                {
                    break;
                }

                temp[index] = new double[columns];

                int j;

                for (j = 0; j < columns; j++)
                {
                    temp[index][j] = table[i][j];
                }

                index++;
            }

            return temp;
        }*/
    }

    public class CalculateX2
    {
        public double N1 { get; set; }

        public double Tkin { get; set; }

        public double N0 { get; set; }

        public double Tcmb { get; set; }

        public double X2 { get; set; }
    }
}