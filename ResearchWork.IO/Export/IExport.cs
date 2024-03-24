using ResearchWork.IO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResearchWork.IO.Export
{
    public interface IExport
    {
        Task ExportTable(IEnumerable<CalculateX2> chi2Table, string exportName);
    }
}