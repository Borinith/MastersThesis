namespace ResearchWork.IO.Export
{
    internal static class ExportTable
    {
        /*public static Task ExportSortedTable(List<CalculateX2> sortedChi2Table, string exportName)
        {
            var task = Task.Run(() =>
            {
                var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                customCulture.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = customCulture;

                // ReSharper disable once ConvertToUsingDeclaration
#pragma warning disable IDE0063 // Use simple 'using' statement
                using (var sw = new StreamWriter(exportName))
#pragma warning restore IDE0063 // Use simple 'using' statement
                {
                    for (var chExport = 0; chExport < sortedChi2Table.Count - 1; chExport++)
                    {
                        sw.WriteLine("{0:0.##}  \t\t{1}  \t{2:0.###} \t\t{3:0.##}  \t\t{4}",
                            sortedChi2Table[chExport].N1,
                            sortedChi2Table[chExport].Tkin,
                            sortedChi2Table[chExport].N0,
                            sortedChi2Table[chExport].Tcmb,
                            sortedChi2Table[chExport].X2);
                    }

                    sw.Write("{0:0.##}  \t\t{1}  \t{2:0.###} \t\t{3:0.##}  \t\t{4}",
                        sortedChi2Table[sortedChi2Table.Count - 1].N1,
                        sortedChi2Table[sortedChi2Table.Count - 1].Tkin,
                        sortedChi2Table[sortedChi2Table.Count - 1].N0,
                        sortedChi2Table[sortedChi2Table.Count - 1].Tcmb,
                        sortedChi2Table[sortedChi2Table.Count - 1].X2);
                }
            });

            return task;
        }*/
    }
}