# TypeHelper

TypeHelper ist eine Windows-Forms-Anwendung (\.NET 7), die als Tipp- und Dokumentationshilfe entwickelt wurde. Sie verfolgt im Hintergrund die Tastatureingaben, schlägt passende Wörter vor und kann Text automatisch ergänzen. Ursprünglich wurde es entwickelt, um Personal zu unterstützen, das im Auftrag einer Kassenärztlichen Vereinigung mithilfe eines separaten Programms medizinische Ersteinschätzungen mit Patientinnen und Patienten durchführte, dokumentierte und anschließend Handlungsempfehlungen aussprach. TypeHelper kam nie produktiv zum Einsatz und wird nicht weiterentwickelt.

## Funktionsüberblick

- **Keylogger**: Ein Low-Level-Keyboard-Hook zeichnet die Tastatureingaben auf.
- **Wortverwaltung**: über CSV-Dateien im Ordner `config/lists` werden Wortlisten und ein PLZ-Verzeichnis geladen. Daraus entstehen sortierte Listen nach Kategorien.
- **Vorschläge & Autokorrektur**: Beim Tippen werden passende Begriffe gesucht (Levenshtein-Algorithmus). Bis zu drei Vorschläge erscheinen in der Oberfläche und können mit `F3` bis `F5` oder per Klick übernommen werden. Optional kann eine automatische Korrektur erfolgen; die maximale Abweichung lässt sich in Prozent festlegen.
- **Wortlisten nach Kategorien**: Über Buttons lassen sich komplette Listen bestimmter Kategorien anzeigen (z. B. Telefonnummern, Medikamente, Fachbegriffe, Bausteine oder PLZ).
- **Textmanipulation**: Per Funktionstaste oder Klick wird das erkannte Wort an der aktuellen Cursorposition eingefügt, zuvor geschriebene Zeichen werden gelöscht.
- **Weitere Optionen**: Ein Ein-Wort-Modus, das Starten/Stoppen der Tastaturaufzeichnung und ein anzeigbarer Disclaimer sind in der Benutzeroberfläche verfügbar.

## Verzeichnisstruktur

- `TypeHelper.ClientApp` – Windows-Forms-Oberfläche
- `TypeHelper.WinApi` – Keylogger und Textmanipulation über Win-API
- `TypeHelper.Words` – Wortlisten, Verwaltung und Levenshtein-Logik
- `TypeHelper.sln` – Lösungsdatei

## Benutzung

1. Die Anwendung erwartet Wortlisten im Verzeichnis `config/lists`. Hier sollten CSV-Dateien mit den benötigten Begriffen liegen (nicht im Repository enthalten).
2. Das Projekt kann mit Visual Studio oder der .NET-CLI gebaut werden (\.NET 7 wird benötigt).

```bash
# Beispiel (wenn dotnet verfügbar ist)
# dotnet build TypeHelper.sln
```

Nach dem Start werden die Listen geladen und die Tastaturaufzeichnung beginnt. Die Vorschläge erscheinen im Hauptfenster und lassen sich per Funktionstasten oder Mausklick einfügen.

## Hinweis

TypeHelper dient ausschließlich als Unterstützung bei der Dokumentation und ersetzt keine medizinische Beratung. Die bereitgestellten Informationen sind allgemeiner Natur.
