using ResearchWork.IO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResearchWork.IO.Export
{
    public interface IExportTable
    {
        Task ExportSortedTable(List<CalculateX2> sortedChi2Table, string exportName);
    }
}