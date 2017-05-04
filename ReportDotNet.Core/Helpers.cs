using System;
using System.Collections.Generic;

namespace ReportDotNet.Core
{
    public static class Helpers
    {
        public static int[] GetWidths(int width,
                                      IEnumerable<int[]> initialWidths)
        {
            var result = new[] { width };
            foreach (var widths in initialWidths)
                result = Merge(result, widths);
            return result;
        }

        public static int[] GetSpans(int[] tableCols,
                                     int[] rowCols)
        {
            var result = new int[rowCols.Length];

            var c = 0;
            for (var i = 0; i < rowCols.Length; i++)
            {
                var rowCol = rowCols[i];
                for (int j = c, span = 1; j < tableCols.Length; j++, span++)
                {
                    rowCol -= tableCols[j];
                    if (rowCol < 0)
                        throw new InvalidOperationException("Неправильно поделились колонки для таблицы");
                    if (rowCol > 0)
                        continue;
                    result[i] = span;
                    c = j + 1;
                    break;
                }
            }

            return result;
        }

        private static int[] Merge(int[] a,
                                   int[] b)
        {
            var ar = new int[a.Length];
            a.CopyTo(ar, 0);
            var br = new int[b.Length];
            b.CopyTo(br, 0);
            var result = new List<int>();
            for (int i = 0, j = 0; i < ar.Length || j < br.Length;)
                if (i == ar.Length)
                {
                    result.Add(br[j]);
                    j++;
                }
                else if (j == br.Length)
                {
                    result.Add(ar[i]);
                    i++;
                }
                else if (ar[i] == br[j])
                {
                    result.Add(ar[i]);
                    i++;
                    j++;
                }
                else if (ar[i] > br[j])
                {
                    result.Add(br[j]);
                    ar[i] -= br[j];
                    j++;
                }
                else
                {
                    result.Add(ar[i]);
                    br[j] -= ar[i];
                    i++;
                }
            return result.ToArray();
        }
    }
}