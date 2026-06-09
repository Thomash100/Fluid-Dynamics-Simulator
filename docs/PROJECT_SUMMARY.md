# Projektzusammenfassung

Stand: 2026-06-09

Repository: https://github.com/Thomash100/Fluid-Dynamics-Simulator

Release: https://github.com/Thomash100/Fluid-Dynamics-Simulator/releases/tag/v0.1.0-alpha

## Projektziel

Fluid Dynamics Simulator (FDS) ist eine Open-Source-Simulationsplattform für technische Gebäudeausrüstung, Hydraulik, Thermodynamik, Luftströmung und BIM-basierte Analyse mit IFC- und Revit-Integration.

## Aktueller Stand

- Repository ist öffentlich.
- Repository-Beschreibung ist gesetzt.
- `version.json` ist gültig und steht auf `0.1.0-alpha`.
- GitHub Actions laufen für `main` und Tags.
- Releases werden über Tags wie `v0.1.0-alpha` erzeugt.
- Der Release `v0.1.0-alpha` ist als Pre-Release markiert und enthält `release.zip`.
- README, MIT-Lizenz, Deployment-Dokumentation und Release-Workflow sind konsistent.
- `FDS.Core` ist als .NET 8 Klassenbibliothek angelegt.
- `FDS.Core.Tests` ist als xUnit-Testprojekt angelegt.
- Die Basismodelle `Node`, `Edge`, `Fluid` und `Network` sind implementiert.
- Ein `Temperature` Value Object trennt Celsius und Kelvin explizit.
- `FDS.Hydraulics` ist als .NET 8 Klassenbibliothek angelegt.
- `FDS.Hydraulics.Tests` ist als xUnit-Testprojekt angelegt.
- Ein Rohrmodell und Einzelrohr-Berechnungen für Geschwindigkeit, Reynoldszahl, Reibungszahl und Darcy-Weisbach-Druckverlust sind implementiert.

## Milestones

| Milestone | Inhalt | Issues |
| --- | --- | --- |
| `v0.1.0-alpha` | Projektbasis, Solution-Grundlage, Beispiele | #2, #3, #13 |
| `v0.2.0-core` | Core-Datenmodell und gemeinsame Abstraktionen | #4, #5 |
| `v0.3.0-hydraulics` | Hydraulischer Solver, Pumpen, Armaturen | #6, #7, #8 |
| `v0.4.0-thermal` | Thermisches Modell und Ergebnisgrundlagen | #9, #10 |
| `v0.5.0-bim` | IFC- und Revit-Grundlagen | #11, #12 |

## Prioritäten

| Priorität | Issues | Begründung |
| --- | --- | --- |
| `priority:p0` | #2, #3, #4, #5 | Struktur, Solution, Fluid und Netzwerkmodell sind Voraussetzung für alle fachlichen Module. |
| `priority:p1` | #6, #7, #8, #13 | Hydraulik ist der erste fachliche Solver; Beispiele sichern Nachvollziehbarkeit. |
| `priority:p2` | #9, #10, #11, #12 | Thermik, Visualisierung und BIM bauen sinnvoll auf Core und Hydraulik auf. |

## Core-Modellstand

| Modell | Status |
| --- | --- |
| `Node` | Implementiert mit ID, optionalem Druck in Pa und optionaler Temperatur. |
| `Edge` | Implementiert mit Knotenreferenzen, Länge in m, Durchmesser in m sowie optionalem Volumen- und Massenstrom. |
| `Fluid` | Implementiert mit ID, Name, Dichte in kg/m³ und optionaler Referenztemperatur. |
| `Network` | Implementiert mit Topologievalidierung, eindeutigen IDs und Knotenreferenzprüfung. |
| `Temperature` | Implementiert mit expliziten Eigenschaften für °C und K. |

## Hydraulik-Modellstand

| Baustein | Status |
| --- | --- |
| `Pipe` | Implementiert mit ID, Länge in m, Innendurchmesser in m, optionalen Knotenreferenzen und Rauheit in m. |
| `PipeFlowCalculator.CalculateVelocityMetersPerSecond` | Implementiert für Einzelrohr-Strömungsgeschwindigkeit aus Volumenstrom und Querschnitt. |
| `PipeFlowCalculator.CalculateReynoldsNumber` | Implementiert mit Dichte, Geschwindigkeit, Durchmesser und dynamischer Viskosität. |
| `PipeFlowCalculator.EstimateDarcyFrictionFactor` | Implementiert mit `64/Re` für laminar und Blasius-Näherung für nicht-laminar. |
| `PipeFlowCalculator.CalculateDarcyWeisbachPressureLossPascals` | Als Einzelrohr-Druckverlust vorbereitet. Kein Netzwerksolver. |

## Einheiten

- Druck: Pa
- Temperatur: °C für Engineering-/Anzeige-Werte, K für absolute thermodynamische Werte
- Volumenstrom: m³/s
- Massenstrom: kg/s
- Länge und Durchmesser: m
- Dichte: kg/m³
- Dynamische Viskosität: Pa·s
- Strömungsgeschwindigkeit: m/s
- Reynoldszahl und Reibungszahl: dimensionslos

## Validierung

- Keine negative Dichte
- Keine negative Kantenlänge
- Kein Durchmesser kleiner oder gleich 0
- Keine leeren IDs
- Eindeutige IDs innerhalb eines Netzwerks
- Keine Kantenreferenzen auf unbekannte Knoten
- Keine Temperaturen unter absolutem Nullpunkt
- Keine negative Rohrrauheit
- Keine dynamische Viskosität kleiner oder gleich 0

## Dokumentation

- `README.md`
- `docs/CORE_MODEL.md`
- `docs/HYDRAULICS_MODEL.md`
- `docs/FDS_CORE_SUMMARY.md`
- `docs/FDS_HYDRAULICS_SUMMARY.md`
- `docs/DEPLOYMENT.md`

## Offener Verwaltungspunkt

Ein GitHub Project Board konnte noch nicht angelegt werden. Die klassische Project-API liefert `404 Not Found`; GitHub Projects v2 benötigt zusätzliche Token-Scopes wie `read:project` beziehungsweise `project`. Die aktuellen Zugangsdaten haben `gist`, `repo` und `workflow`.

## Empfohlener nächster Entwicklungsschritt

Als nächster technischer Schritt sollte `FDS.Hydraulics` um einfache Beispielrohre und dokumentierte Referenzfälle ergänzt werden. Danach kann die eigentliche Netzsolver-Architektur separat geplant werden.
