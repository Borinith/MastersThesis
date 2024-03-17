using ResearchWork.Application.Utils;
using ResearchWork.Calculation;
using ResearchWork.Calculation.CalculationCO;
using ResearchWork.IO.Input;
using ResearchWork.IO.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace ResearchWork.Application.StartCalculation
{
    public class StartCalculation : IStartCalculation
    {
        private readonly ICalculationX2 _calculationX2;
        private ConcurrentBag<CalculateX2>? _chi2Table;

        public StartCalculation(ICalculationX2 calculationX2)
        {
            _calculationX2 = calculationX2;
        }

        public Task<CalculateX2[]> CalculationX2Table(
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

                _chi2Table = new ConcurrentBag<CalculateX2>();

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

                                    var chi2TableRow = _calculationX2.CalculateX2(inputParameters, n0, nCopy, kinCopy, cmbCopy, coeffCopy, rotationLevelsPrG);

                                    _chi2Table.Add(chi2TableRow);
                                });

                            if (cancellationToken.IsCancellationRequested)
                            {
                                return Array.Empty<CalculateX2>();
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

                var chi2MinPlus1 = _chi2Table.Any()
                    ? _chi2Table.Min(x => x.X2) + 1
                    : 1;

                return _chi2Table
                    .Where(x => x.X2 <= chi2MinPlus1)
                    .OrderBy(x => x.X2)
                    .ToArray();
            }, cancellationToken);
        }

        public void Dispose()
        {
            _chi2Table = null;

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
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