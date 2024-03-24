using ResearchWork.IO.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResearchWork.IO.Export
{
    public class Export : IExport
    {
        public Task ExportTable(IEnumerable<CalculateX2> chi2Table, string exportName)
        {
            return Task.Run(() =>
            {
                var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                customCulture.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = customCulture;

                var sb = new StringBuilder();

                foreach (var chi2Row in chi2Table)
                {
                    sb.AppendLine($"{chi2Row.N:0.##}  \t\t{chi2Row.Tkin}  \t{chi2Row.N0:0.###} \t\t{chi2Row.Tcmb:0.##}  \t\t{chi2Row.X2}");
                }

                var newLineCount = Environment.NewLine.Length;

                if (sb.Length >= newLineCount)
                {
                    sb.Length -= newLineCount;
                }

                using var sw = new StreamWriter(exportName);
                sw.Write(sb);
            });
        }
    }
}