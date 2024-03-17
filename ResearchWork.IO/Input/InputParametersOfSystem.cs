// Ignore Spelling: Cmb

namespace ResearchWork.IO.Input
{
    public class InputParametersOfSystem
    {
        public double[] DRotationLevelsPr = null!;

        public double[] RotationLevelsPr = null!;

        public string SystemName { get; set; } = null!;

        public string ExportName { get; set; } = null!;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public double Z { get; set; }

        public int NumberOfLevels { get; set; }

        public double F { get; set; }

        public decimal TemperatureKinPr { get; set; }

        public decimal DTemperatureKinMin { get; set; }

        public decimal DTemperatureKinMax { get; set; }

        public decimal NPr { get; set; }

        public decimal DnMin { get; set; }

        public decimal DnMax { get; set; }

        public decimal NMin { get; set; }

        public decimal NMax { get; set; }

        public decimal NStep { get; set; }

        public int NRound { get; set; }

        public decimal TemperatureKinMin { get; set; }

        public decimal TemperatureKinMax { get; set; }

        public decimal TemperatureKinStep { get; set; }

        public int TemperatureKinRound { get; set; }

        public decimal TemperatureCmbMin { get; set; }

        public decimal TemperatureCmbMax { get; set; }

        public decimal TemperatureCmbStep { get; set; }

        public int TemperatureCmbRound { get; set; }

        public decimal N0Min { get; set; }

        public decimal N0Max { get; set; }

        public decimal N0Step { get; set; }

        public int N0Round { get; set; }
    }
}