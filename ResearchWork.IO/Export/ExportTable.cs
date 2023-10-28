using ResearchWork.IO.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResearchWork.IO.Export
{
    public class ExportTable : IExportTable
    {
        public Task ExportSortedTable(List<CalculateX2> sortedChi2Table, string exportName)
        {
            return Task.Run(() =>
            {
                var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                customCulture.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = customCulture;

                var sb = new StringBuilder();

                if (sortedChi2Table.Any())
                {
                    for (var chExport = 0; chExport < sortedChi2Table.Count - 1; chExport++)
                    {
                        sb.AppendLine($"{sortedChi2Table[chExport].N:0.##}  \t\t{sortedChi2Table[chExport].Tkin}  \t{sortedChi2Table[chExport].N0:0.###} \t\t{sortedChi2Table[chExport].Tcmb:0.##}  \t\t{sortedChi2Table[chExport].X2}");
                    }

                    sb.Append($"{sortedChi2Table.LastOrDefault().N:0.##}  \t\t{sortedChi2Table.LastOrDefault().Tkin}  \t{sortedChi2Table.LastOrDefault().N0:0.###} \t\t{sortedChi2Table.LastOrDefault().Tcmb:0.##}  \t\t{sortedChi2Table.LastOrDefault().X2}");
                }

                using var sw = new StreamWriter(exportName);
                sw.Write(sb);
            });
        }
    }
}