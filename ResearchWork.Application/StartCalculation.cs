using ResearchWork.Application.Utils;
using ResearchWork.Calculation;
using ResearchWork.Calculation.CalculationCO;
using ResearchWork.IO.Input;
using ResearchWork.IO.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ResearchWork.Application
{
    public static class StartCalculation
    {
        public static Task<List<CalculateX2>> CalculationX2Table(
            InputParametersOfSystem inputParameters,
            IProgress<double> progress,
            IProgress<TimeSpan> timeProgress,
            CancellationToken cancellationToken)
        {
            return Task.Run(() =>
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
                            var kinCopy = temperatureKin;
                            var cmbCopy = temperatureCmb;
                            var coeffCopy = alphaParaCoeff;
                            var nCopy = n;

                            Parallel.For(
                                (int)Math.Round(inputParameters.N0Min / inputParameters.N0Step, 0),
                                (int)Math.Round(inputParameters.N0Max / inputParameters.N0Step, 0) + 1,
                                (n0Big, state) =>
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        state.Stop();

                                        return;
                                    }

                                    var n0 = Math.Round(n0Big * inputParameters.N0Step, inputParameters.N0Round);

                                    var chi2TableRow = CalculationX2.CalculateX2(inputParameters, n0, nCopy, kinCopy, cmbCopy, coeffCopy, rotationLevelsPrG);

                                    lock (chi2TableList)
                                    {
                                        chi2TableList.Add(chi2TableRow);
                                    }
                                });

                            if (cancellationToken.IsCancellationRequested)
                            {
                                return new List<CalculateX2>();
                            }

                            chNumberOfX2 += (int)((inputParameters.N0Max -
                                                   inputParameters.N0Min +
                                                   inputParameters.N0Step) /
                                                  inputParameters.N0Step);

                            progress.Report((double)chNumberOfX2 / lenChi2Table * 100);

                            SetProgress.SetProgressValue(chNumberOfX2, lenChi2Table);

                            timeProgress.Report(sw.GetEta(chNumberOfX2, lenChi2Table));
                        }
                    }
                }

                var sortedChi2Table = chi2TableList.OrderBy(x => x.X2).ToList();

                var chi2MinPlus1 = sortedChi2Table.FirstOrDefault()?.X2 + 1;

                return sortedChi2Table.Where(x => x.X2 <= chi2MinPlus1).ToList();
            }, cancellationToken);
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
    }
}