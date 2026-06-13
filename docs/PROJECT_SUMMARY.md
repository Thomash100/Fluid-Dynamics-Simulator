# Projektzusammenfassung

Stand: 2026-06-13

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
- Armaturen-/Einzelwiderstandsmodelle für `LocalResistance`, `Fitting` und `Valve` sind implementiert.
- Zeta-basierter Druckverlust und ein Kv/Kvs-Grundmodell sind vorbereitet.
- Ein Pumpen-Grundmodell mit Kennlinie, Förderhöheninterpolation, hydraulischer Leistung und optionaler Wirkungsgradkennlinie ist implementiert.
- Eine einfache hydraulische Strangberechnung aggregiert Rohrverluste, Einzelwiderstände und Pumpen-Druckerhöhung bei vorgegebenem Volumenstrom.
- Eine feste hydraulische Netzwerkauswertung für mehrere Stränge mit bekannten Volumenströmen ermittelt BranchResults, den ungünstigsten Strang und die erforderliche Mindest-Pumpendruckerhöhung.
- Die Vorbereitung für einen iterativen hydraulischen Netzwerksolver enthält Solver-Optionen, Randbedingungen, Knotenbilanzen und Druckresiduen.
- Ein erster kleiner iterativer Referenzsolver wertet vorbereitete Netze mit einfacher Relaxation aus und dokumentiert den Iterationsverlauf.
- Eine konfigurierbare WinForms-Test-App unter `samples/FDS.WindowsApp` prüft den Referenzsolver lokal mit deutschen UI-Texten sowie Eingaben für Druckdifferenz, Rohrdurchmesser, Zeta-Werte und Gesamtvolumenstrom.
- Der erste Solver-Core-Zwischenstand ist auf `main` integriert; die Windows-App wird zusätzlich über einen CI-Smoke-Test geprüft.

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
| `LocalResistance` | Implementiert mit dimensionslosem Zeta-Wert. |
| `Fitting` | Implementiert als Armaturen-/Formstückmodell mit Fitting-Art und Zeta-Widerstand. |
| `Valve` | Implementiert als Ventil-Grundmodell mit optionalem Zeta- und Kv/Kvs-Datensatz. |
| `LocalResistanceCalculator` | Implementiert für Zeta-Druckverlust und Kv-basierten Ventil-Druckverlust. |
| `Pump` | Implementiert als Pumpen-Grundmodell mit Förderhöhenkennlinie und optionaler Wirkungsgradkennlinie. |
| `PumpCurve` | Implementiert mit sortierten, eindeutigen Volumenstrom/Förderhöhe-Stützpunkten und linearer Interpolation. |
| `PumpEfficiencyCurve` | Implementiert als optionales Wirkungsgrad-Grundmodell mit linearer Interpolation. |
| `PumpCalculator` | Implementiert für Förderhöhenabfrage, hydraulische Leistung, Druckerhöhung und optionale Wellenleistung. |
| `HydraulicBranch` | Implementiert als einfacher Strang aus Rohren, lokalen Widerständen, Armaturen, Ventilen und optionaler Pumpe. |
| `HydraulicBranchResult` | Implementiert als Ergebnisobjekt für Volumenstrom, Verlustsummen, Pumpendruckerhöhung und Netto-Druckbilanz. |
| `HydraulicBranchCalculator` | Implementiert für Strangberechnung bei fest vorgegebenem Volumenstrom. Kein Betriebspunktsolver. |
| `HydraulicBranchFlow` | Implementiert als Zuordnung von Strang zu bekanntem Volumenstrom. |
| `HydraulicNetwork` | Implementiert als feste Netzwerkauswertung aus mehreren Strängen mit bekannten Volumenströmen. |
| `HydraulicNetworkBranchResult` | Implementiert als Netzwerk-Ergebnis für einen einzelnen Strang. |
| `HydraulicNetworkResult` | Implementiert mit allen BranchResults, kritischem Strang, erforderlicher Pumpendruckerhöhung und optionaler Förderhöhe. |
| `HydraulicNetworkCalculator` | Implementiert für Netzwerkauswertung ohne Volumenstromabgleich, Solver oder Iteration. |
| `HydraulicSolverOptions` | Implementiert mit Iterationslimit, Flow-Residualtoleranz, Druck-Residualtoleranz und Relaxationsfaktor. |
| `HydraulicBoundaryCondition` | Implementiert für Quelle, Senke, bekannten Druck, bekannte Druckdifferenz und Pumpenkennlinie. |
| `HydraulicNodeBalance` | Implementiert mit Residualkonvention `Q_in + Q_source - Q_out - Q_sink`. |
| `HydraulicPressureResidual` | Implementiert mit Residualkonvention `dp_available - dp_required`. |
| `HydraulicSolverResult` | Implementiert für vorbereitete Residualdaten mit Status und Iterationszahl. |
| `HydraulicSolverPreparationCalculator` | Implementiert für Knotenbilanz- und Druckresidualauswertung ohne Iteration. |
| `IHydraulicNetworkSolver` | Implementiert als minimale Solver-Schnittstelle. |
| `HydraulicSolverInput` | Implementiert als Eingabeobjekt für Topologie, Branches, Randbedingungen, Startwerte und Optionen. |
| `HydraulicSolverIteration` | Implementiert als Iterationssnapshot mit Flow-Schätzungen und Residuen. |
| `SmallHydraulicNetworkSolver` | Implementiert als kleiner Relaxationssolver für vorbereitete Referenznetze. Kein allgemeiner Netzsolver. |

## Windows-App-Teststand

| Baustein | Status |
| --- | --- |
| `samples/FDS.WindowsApp` | Implementiert als WinForms-Test-Harness für den kleinen Referenzsolver mit deutschen UI- und Ergebnistexten. |
| Eingaben | Druckdifferenz in Pa, Rohrdurchmesser in m, Zeta-Werte für zwei parallele Stränge und Gesamtvolumenstrom in m³/s. |
| Szenario-Presets | Referenzfall, höhere Druckdifferenz und engeres Rohr als schnelle Vergleichsfälle. |
| Ergebnisanzeige | Übersicht, Status, Iterationszahl, Knotenbilanz-Residual, Druck-Residual, Ergebnisbewertung, Prüfhinweise, Strang-Volumenströme, Druckresiduen, Iterationstabelle, Preset-Vergleich und Textausgabe. |
| Smoke-Test | Implementiert über `FDS.WindowsApp.exe --smoke-test`; der Referenzfall konvergiert mit Residuen 0. |
| Abgrenzung | Keine produktive UI und keine zusätzliche Solver-Logik in der App. |

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
- Zeta-Wert: dimensionslos
- Kv/Kvs: m³/h
- Pumpen-Förderhöhe: m
- Pumpenleistung: W
- Pumpenwirkungsgrad: dimensionslos, 0 < eta <= 1
- Strang-Druckbilanz: Pa
- Erforderliche Pumpendruckerhöhung: Pa
- Erforderliche Förderhöhe: m
- Knotenbilanz-Residual: m³/s
- Druck-Residual: Pa

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
- Keine negativen Zeta-Werte
- Kein Kv oder Kvs kleiner oder gleich 0
- Kein Kv größer als Kvs
- Keine leeren Pumpen-IDs oder Pumpennamen
- Keine Pumpenkennlinien mit weniger als zwei Stützpunkten
- Keine doppelten Volumenstromwerte in Pumpenkennlinien
- Keine negative Förderhöhe
- Keine mit steigendem Volumenstrom steigende Förderhöhe
- Kein Pumpenwirkungsgrad kleiner oder gleich 0 oder größer als 1
- Kein hydraulischer Strang ohne Rohr
- Kein negativer Volumenstrom in der einfachen Strangberechnung
- Kein hydraulisches Netzwerk ohne Strang
- Keine doppelten Strang-IDs innerhalb eines hydraulischen Netzwerks
- Kein negativer vorgegebener Netzwerk-Volumenstrom
- Keine Solver-Optionen mit ungültigen Toleranzen oder Relaxationsfaktoren
- Keine Solver-Randbedingungen auf unbekannte Knoten
- Kein kleiner Solverlauf ohne topologisch referenzierte Branch-Endpunkte

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

Als nächster technischer Schritt sollte der kleine Referenzsolver anhand weiterer hydraulischer Referenzfälle kalibriert werden. Erst danach sollte eine zweite Solver-Variante wie ein Newton- oder Gradientenverfahren ergänzt werden.
