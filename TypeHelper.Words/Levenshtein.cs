using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelper.Words;

public class Levenshtein
{
    public static int LevenshteinDistance(string source, string target, int maxDistance, bool disableBeginningPrivilege = false)
    {
        // Konvertiere beide Strings in Kleinbuchstaben, um Groß-/Kleinschreibung zu ignorieren
        source = source.ToLowerInvariant();
        target = target.ToLowerInvariant();

        int n = source.Length;
        int m = target.Length;

        // Wenn der String mit dem anderen beginnt
        if (source.StartsWith(target) && !disableBeginningPrivilege)
            return 0;

        // Überprüfe, ob der Unterschied in den Längen der Strings größer als maxDistance ist
        if (Math.Abs(n - m) > maxDistance)
            return maxDistance + 1;

        // Wenn einer der Strings leer ist, ist die Distanz die Länge des anderen Strings
        if (n == 0) return m > maxDistance ? maxDistance + 1 : m;
        if (m == 0) return n > maxDistance ? maxDistance + 1 : n;

        // Erzeuge ein Array zur Speicherung der aktuellen Distanzwerte
        int[] previousRow = new int[m + 1];
        int[] currentRow = new int[m + 1];

        // Initialisiere die erste Zeile (source auf leeren target transformieren)
        for (int j = 0; j <= m; j++)
            previousRow[j] = j;

        for (int i = 1; i <= n; i++)
        {
            currentRow[0] = i;
            int minInRow = int.MaxValue;

            for (int j = 1; j <= m; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;
                currentRow[j] = Math.Min(
                    Math.Min(previousRow[j] + 1, currentRow[j - 1] + 1),
                    previousRow[j - 1] + cost
                );

                minInRow = Math.Min(minInRow, currentRow[j]);
            }

            // Wenn der Minimalwert der aktuellen Zeile den Schwellwert überschreitet, breche ab
            if (minInRow > maxDistance)
                return maxDistance + 1;

            // Tausche die Zeilenarrays
            var temp = previousRow;
            previousRow = currentRow;
            currentRow = temp;
        }

        // Die letzte Zelle enthält die Levenshtein-Distanz
        return previousRow[m];
    }
}