namespace ResearchWork3.Input
{
    public class InitialData
    {
        private readonly double[] _bb;

        public InitialData(InputParametersOfSystem inputParameters)
        {
            InputParameters = inputParameters;

            _bb = new double[InputParameters.NumberOfLevels + 1];
            GetBb()[InputParameters.NumberOfLevels] = 1;
        }

        public InputParametersOfSystem InputParameters { get; }

        public double[] GetBb()
        {
            return _bb;
        }
    }
}