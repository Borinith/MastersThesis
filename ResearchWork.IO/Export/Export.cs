using ResearchWork.IO.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

                using var sw = new StreamWriter(exportName);

                for (var chExport = 0; chExport < sortedChi2Table.Count - 1; chExport++)
                {
                    sw.WriteLine("{0:0.##}  \t\t{1}  \t{2:0.###} \t\t{3:0.##}  \t\t{4}",
                        sortedChi2Table[chExport].N,
                        sortedChi2Table[chExport].Tkin,
                        sortedChi2Table[chExport].N0,
                        sortedChi2Table[chExport].Tcmb,
                        sortedChi2Table[chExport].X2);
                }

                sw.Write("{0:0.##}  \t\t{1}  \t{2:0.###} \t\t{3:0.##}  \t\t{4}",
                    sortedChi2Table.LastOrDefault()?.N,
                    sortedChi2Table.LastOrDefault()?.Tkin,
                    sortedChi2Table.LastOrDefault()?.N0,
                    sortedChi2Table.LastOrDefault()?.Tcmb,
                    sortedChi2Table.LastOrDefault()?.X2);
            });
        }
    }
}