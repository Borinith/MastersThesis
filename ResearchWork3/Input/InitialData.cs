using System.Linq;

namespace ResearchWork3.Input
{
    public class InitialData
    {
        private readonly double[] _bb;

        public InitialData(InputParametersOfSystem inputParameters)
        {
            InputParameters = inputParameters;

            _bb = Enumerable.Repeat(0d, InputParameters.NumberOfLevels + 1).ToArray();
            _bb[InputParameters.NumberOfLevels] = 1;
        }

        public InputParametersOfSystem InputParameters { get; }

        public double[] GetBb()
        {
            return _bb;
        }
    }
}