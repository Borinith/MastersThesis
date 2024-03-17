using ResearchWork.IO.Models;
using System.Threading.Tasks;

namespace ResearchWork.IO.Export
{
    public interface IExportTable
    {
        Task ExportSortedTable(CalculateX2[] sortedChi2Table, string exportName);
    }
}