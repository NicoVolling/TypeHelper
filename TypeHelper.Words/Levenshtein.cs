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

        //Wenn der String mit dem anderen Beginnt
        if (source.StartsWith(target) && !disableBeginningPrivilege)
        {
            return 0;
        }

        // Wenn einer der Strings leer ist, ist die Distanz die Länge des anderen Strings
        if (n == 0)
        {
            return m > maxDistance ? maxDistance + 1 : m;
        }

        if (m == 0)
        {
            return n > maxDistance ? maxDistance + 1 : n;
        }

        // Erzeuge ein Array zur Speicherung der vorherigen und aktuellen Distanzwerte
        int[] previousRow = new int[m + 1];
        int[] currentRow = new int[m + 1];

        // Initialisiere die erste Zeile (source auf leeren target transformieren)
        for (int j = 0; j <= m; j++)
        {
            previousRow[j] = j;
        }

        for (int i = 1; i <= n; i++)
        {
            currentRow[0] = i;

            int minInRow = int.MaxValue; // Um den Minimalwert der aktuellen Zeile zu verfolgen

            for (int j = 1; j <= m; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;

                // Berechne die minimale Distanz unter den Operationen: Einfügen, Löschen, Ersetzen
                currentRow[j] = Math.Min(
                    Math.Min(previousRow[j] + 1, currentRow[j - 1] + 1),
                    previousRow[j - 1] + cost
                );

                // Aktualisiere den Minimalwert in der aktuellen Zeile
                minInRow = Math.Min(minInRow, currentRow[j]);
            }

            // Kopiere die aktuelle Zeile in die vorherige Zeile für den nächsten Durchlauf
            Array.Copy(currentRow, previousRow, m + 1);

            // Wenn der Minimalwert der aktuellen Zeile den Schwellwert überschreitet, breche ab
            if (minInRow > maxDistance)
            {
                return maxDistance + 1;
            }
        }

        // Die letzte Zelle enthält die Levenshtein-Distanz
        return currentRow[m];
    }
}