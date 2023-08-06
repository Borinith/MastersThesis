namespace ResearchWork.IO.Models
{
    public record CalculateX2
    {
        public decimal N { get; init; }

        public decimal Tkin { get; init; }

        public decimal N0 { get; init; }

        public decimal Tcmb { get; init; }

        public double X2 { get; init; }
    }
}