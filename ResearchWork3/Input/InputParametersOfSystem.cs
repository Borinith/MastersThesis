namespace ResearchWork3.Input
{
    public class InputParametersOfSystem
    {
        public double[] DRotationLevelsPr;

        public double[] RotationLevelsPr;

        public string SystemName { get; set; }

        public string ExportName { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public double Z { get; set; }

        public int NumberOfLevels { get; set; }

        public double F { get; set; }

        public double TemperatureKinPr { get; set; }

        public double DTemperatureKinMin { get; set; }

        public double DTemperatureKinMax { get; set; }

        public double NPr { get; set; }

        public double DnMin { get; set; }

        public double DnMax { get; set; }

        public double NMin { get; set; }

        public double NMax { get; set; }

        public double NStep { get; set; }

        public int NRound { get; set; }

        public double TemperatureKinMin { get; set; }

        public double TemperatureKinMax { get; set; }

        public double TemperatureKinStep { get; set; }

        public int TemperatureKinRound { get; set; }

        public double TemperatureCmbMin { get; set; }

        public double TemperatureCmbMax { get; set; }

        public double TemperatureCmbStep { get; set; }

        public int TemperatureCmbRound { get; set; }

        public double N0Min { get; set; }

        public double N0Max { get; set; }

        public double N0Step { get; set; }

        public int N0Round { get; set; }
    }
}