using ResearchWork.IO.Input;
using ResearchWork.IO.Models;
using System.Collections.Generic;

namespace ResearchWork.Calculation.CalculationCO
{
    public interface ICalculationX2
    {
        CalculateX2 CalculateX2(
            InputParametersOfSystem inputParameters,
            decimal n0,
            decimal n,
            decimal temperatureKin,
            decimal temperatureCmb,
            double alphaParaCoeff,
            IReadOnlyList<double> rotationLevelsPrG);
    }
}