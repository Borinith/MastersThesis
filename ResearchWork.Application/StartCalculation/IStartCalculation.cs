using ResearchWork.IO.Input;
using ResearchWork.IO.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ResearchWork.Application.StartCalculation
{
    public interface IStartCalculation
    {
        Task<List<CalculateX2>> CalculationX2Table(
            InputParametersOfSystem inputParameters,
            IProgress<double> progress,
            IProgress<TimeSpan> timeProgress,
            CancellationToken cancellationToken);
    }
}