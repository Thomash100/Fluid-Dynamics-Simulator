# Fluid Dynamics Simulator (FDS)

Fluid Dynamics Simulator (FDS) ist eine Simulationsplattform für technische Gebäudeausrüstung (TGA), Hydraulik, Thermodynamik und Strömungstechnik.

## Ziele

- Hydraulische Netzberechnung
- Thermische Simulation
- Lufttechnische Netzberechnung
- Raumströmungssimulation
- IFC-Integration
- Revit-Integration
- BIM-basierte Analyse

## Aktueller Stand

Die erste Grundstruktur für `FDS.Core` ist vorhanden. Sie enthält Basismodelle für Netzwerke, Knoten, Kanten und Fluide sowie Unit Tests für die grundlegenden Validierungen.

`FDS.Hydraulics` ist als nächstes Modul angelegt. Es enthält ein Rohrmodell, Einzelrohr-Berechnungen für Strömungsgeschwindigkeit, Reynoldszahl, einfache Reibungszahl-Abschätzung und vorbereiteten Darcy-Weisbach-Druckverlust. Zusätzlich sind Armaturen-/Einzelwiderstandsmodelle mit Zeta-Druckverlust sowie ein Kv/Kvs-Grundmodell für Ventile vorbereitet. Ein Pumpen-Grundmodell mit Kennlinien-Stützpunkten, linearer Förderhöheninterpolation, hydraulischer Leistung und optionaler Wirkungsgradkennlinie ist ebenfalls enthalten. Eine einfache Strangberechnung aggregiert diese Bausteine bei vorgegebenem Volumenstrom. Eine feste Netzwerkauswertung fasst mehrere Stränge mit bekannten Volumenströmen zusammen und ermittelt den ungünstigsten Strang sowie die erforderliche Mindest-Pumpendruckerhöhung. Zusätzlich ist ein erster kleiner iterativer Referenzsolver für vorbereitete Netzwerke enthalten. Er nutzt Knotenbilanz- und Druckresiduen, ist aber noch kein allgemeiner Netzwerksolver.

Unter `samples/FDS.WindowsApp` liegt eine kleine WinForms-Test-App für den Referenzsolver. Sie dient nur als konfigurierbares Test-Harness, ist keine produktive Oberfläche und verwendet derzeit deutsche UI-Texte. Die App enthält Presets, einen Preset-Vergleich, strukturierte Ergebnis-Tabs, eine Ergebnisbewertung, Prüfhinweise, Tabellen für Strang-Volumenströme, Druckresiduen und Iterationen sowie weiterhin eine Textausgabe für Smoke-Tests und Debugging. Die Eingabe- und Ergebniseinheiten können im Menü `Einstellungen` → `Einheiten` für Druck, Länge/Durchmesser und Volumenstrom umgeschaltet werden; intern bleiben Solver und Modellfelder in SI-Einheiten.

## Projektstruktur

```text
src/
  FDS.Core/
    Models/
      Edge.cs
      Fluid.cs
      Network.cs
      Node.cs
      Temperature.cs
  FDS.Hydraulics/
    Calculations/
      HydraulicBranchCalculator.cs
      HydraulicNetworkCalculator.cs
      HydraulicSolverPreparationCalculator.cs
      IHydraulicNetworkSolver.cs
      LocalResistanceCalculator.cs
      PipeFlowCalculator.cs
      PumpCalculator.cs
      SmallHydraulicNetworkSolver.cs
    Models/
      Fitting.cs
      HydraulicBranch.cs
      HydraulicBranchFlow.cs
      HydraulicBranchResult.cs
      HydraulicBoundaryCondition.cs
      HydraulicBoundaryConditionKind.cs
      HydraulicNetwork.cs
      HydraulicNetworkBranchResult.cs
      HydraulicNetworkResult.cs
      HydraulicNodeBalance.cs
      HydraulicPressureResidual.cs
      HydraulicSolverInput.cs
      HydraulicSolverIteration.cs
      HydraulicSolverOptions.cs
      HydraulicSolverResult.cs
      HydraulicSolverStatus.cs
      LocalResistance.cs
      Pipe.cs
      Pump.cs
      PumpCurve.cs
      PumpCurvePoint.cs
      PumpEfficiencyCurve.cs
      PumpEfficiencyPoint.cs
      Valve.cs
      ValveFlowCoefficient.cs

tests/
  FDS.Core.Tests/
  FDS.Hydraulics.Tests/

samples/
  FDS.WindowsApp/
```

## Core-Basismodelle

- `Node`: Netzwerkknoten mit eindeutiger ID, optionalem Druck in Pa und optionaler Temperatur.
- `Edge`: gerichtete Verbindung zwischen zwei Knoten mit Länge, Durchmesser und optionalen Strömungsgrößen.
- `Fluid`: Fluiddefinition mit Dichte und optionaler Referenztemperatur.
- `Network`: validierte Topologie aus Knoten, Kanten und optionalem Fluid.
- `Temperature`: Value Object für die saubere Trennung von Celsius und Kelvin.

## Hydraulik-Grundlagen

- `Pipe`: Rohrsegment mit Länge, Innendurchmesser, optionaler Knotenreferenz und Rauheit.
- `PipeFlowCalculator`: Einzelrohr-Hilfsrechnungen ohne Netzwerk-Solver.
- Strömungsgeschwindigkeit aus Volumenstrom und Rohrquerschnitt.
- Reynoldszahl aus Dichte, Geschwindigkeit, Durchmesser und dynamischer Viskosität.
- Darcy-Reibungszahl: laminar `64/Re`, außerhalb des laminaren Bereichs einfache Blasius-Näherung.
- Darcy-Weisbach-Druckverlust als vorbereitete Einzelrohrrechnung.
- `LocalResistance`: Einzelwiderstand mit dimensionslosem Zeta-Wert.
- `Fitting`: Armaturen-/Formstück-Grundmodell auf Basis eines Zeta-Werts.
- `Valve`: Ventil-Grundmodell mit optionalem Zeta-Wert und optionalem Kv/Kvs-Datensatz.
- Zeta-basierter Druckverlust als Einzelkomponentenrechnung.
- Kv/Kvs-basierter Ventil-Druckverlust nach metrischer Kv-Konvention.
- `Pump`: Pumpen-Grundmodell mit Förderhöhenkennlinie und optionaler Wirkungsgradkennlinie.
- `PumpCurve`: Kennlinie aus Volumenstrom/Förderhöhe-Stützpunkten mit linearer Interpolation.
- `PumpCalculator`: Einzelpumpen-Hilfsrechnungen für Förderhöhe, hydraulische Leistung und optionale Wellenleistung.
- `HydraulicBranch`: einfacher hydraulischer Strang aus Rohren, Einzelwiderständen, Armaturen, Ventilen und optionaler Pumpe.
- `HydraulicBranchCalculator`: Druckbilanz bei vorgegebenem Volumenstrom ohne Iteration und ohne Netzwerksolver.
- `HydraulicNetwork`: feste Netzwerkauswertung aus mehreren Strängen mit bekannten Volumenströmen.
- `HydraulicNetworkCalculator`: wertet alle Stränge aus, bestimmt den ungünstigsten Strang und die erforderliche Mindest-Pumpendruckerhöhung.
- `HydraulicBoundaryCondition`: Randbedingungen für Quelle, Senke, bekannten Druck, bekannte Druckdifferenz oder Pumpenkennlinie.
- `HydraulicNodeBalance`: Knotenbilanz mit Residualwert für Volumenstrom.
- `HydraulicPressureResidual`: Druckresidual für spätere Edge-/Stranggleichungen.
- `HydraulicSolverPreparationCalculator`: bereitet Knoten- und Druckresiduen für einen späteren iterativen Solver vor.
- `IHydraulicNetworkSolver`: minimale Solver-Schnittstelle.
- `HydraulicSolverInput`: Eingabeobjekt für vorbereitete kleine Netze, Randbedingungen und Startwerte.
- `HydraulicSolverIteration`: dokumentiert Flüsse und Residuen je Iteration.
- `SmallHydraulicNetworkSolver`: einfacher Relaxationssolver für kleine Referenznetze.

## Einheiten

| Größe | Einheit | Modellfeld |
| --- | --- | --- |
| Druck | Pa | `Node.PressurePascals` |
| Temperatur | °C und K | `Temperature.Celsius`, `Temperature.Kelvin` |
| Volumenstrom | m³/s | `Edge.VolumetricFlowRateCubicMetersPerSecond` |
| Massenstrom | kg/s | `Edge.MassFlowRateKilogramsPerSecond` |
| Länge | m | `Edge.LengthMeters` |
| Durchmesser | m | `Edge.DiameterMeters` |
| Dichte | kg/m³ | `Fluid.DensityKilogramsPerCubicMeter` |
| Dynamische Viskosität | Pa·s | Parameter `dynamicViscosityPascalSeconds` |
| Strömungsgeschwindigkeit | m/s | Ergebnis `CalculateVelocityMetersPerSecond` |
| Reynoldszahl | dimensionslos | Ergebnis `CalculateReynoldsNumber` |
| Druckverlust | Pa | Ergebnis `CalculateDarcyWeisbachPressureLossPascals` |
| Zeta-Wert | dimensionslos | `LocalResistance.Zeta` |
| Kv/Kvs | m³/h | `ValveFlowCoefficient` |
| Pumpen-Förderhöhe | m | `PumpCurvePoint.HeadMeters` |
| Pumpenleistung | W | Ergebnis `CalculateHydraulicPowerWatts` |
| Wirkungsgrad | dimensionslos | `PumpEfficiencyPoint.Efficiency` |
| Netto-Druckbilanz | Pa | `HydraulicBranchResult.NetPressureBalancePascals` |
| Erforderliche Pumpendruckerhöhung | Pa | `HydraulicNetworkResult.RequiredPumpPressureIncreasePascals` |
| Erforderliche Förderhöhe | m | `HydraulicNetworkResult.RequiredPumpHeadMeters` |
| Knotenbilanz-Residual | m³/s | `HydraulicNodeBalance.ResidualFlowCubicMetersPerSecond` |
| Druck-Residual | Pa | `HydraulicPressureResidual.ResidualPressurePascals` |

## Validierungen

- IDs dürfen nicht leer sein.
- IDs in einem `Network` müssen eindeutig sein.
- Kanten dürfen nur vorhandene Knoten referenzieren.
- Fluiddichte darf nicht negativ sein.
- Kantenlänge darf nicht negativ sein.
- Kantendurchmesser muss größer als 0 sein.
- Temperatur in K darf nicht negativ sein.
- Rohrlänge darf nicht negativ sein.
- Rohrinnendurchmesser muss größer als 0 sein.
- Rohrrauheit darf nicht negativ sein.
- Dynamische Viskosität muss größer als 0 sein.
- Zeta-Werte dürfen nicht negativ sein.
- Kv und Kvs müssen größer als 0 sein.
- Kv darf nicht größer als Kvs sein.
- Pumpen-IDs und -Namen dürfen nicht leer sein.
- Pumpenkennlinien benötigen mindestens zwei Stützpunkte.
- Volumenstrom- und Förderhöhen-Stützpunkte dürfen nicht negativ sein.
- Förderhöhe darf mit steigendem Volumenstrom nicht steigen.
- Wirkungsgrade müssen größer als 0 und kleiner oder gleich 1 sein.
- Hydraulische Stränge benötigen mindestens ein Rohr.
- Strangberechnungen akzeptieren aktuell nur nichtnegative Volumenströme in m³/s.
- Hydraulische Netzwerke benötigen mindestens einen Strang.
- Strang-IDs müssen innerhalb eines hydraulischen Netzwerks eindeutig sein.
- Vorgegebene Netzwerk-Volumenströme dürfen nicht negativ sein.
- Solver-Optionen benötigen positive Toleranzen und eine Relaxation im Bereich 0 < r <= 1.
- Solver-Randbedingungen dürfen nur bekannte Knoten referenzieren.
- Kleine Solver-Netze benötigen topologisch referenzierte Branch-Endpunkte.

## Nicht enthalten

- Kein allgemeiner hydraulischer Netzwerksolver
- Kein automatischer Volumenstromabgleich
- Keine automatische Pumpen-Betriebspunktberechnung
- Keine Newton-, Hardy-Cross- oder Gradient-Iteration
- Keine Pumpenkennlinienauswahl
- Keine Pumpenregelstrategie
- Keine vollständige Regelventil-Auslegung
- Keine produktive UI; nur eine WinForms-Test-App als lokales Solver-Test-Harness
- Keine IFC- oder Revit-Schnittstelle

## Geplante Module

- FDS.Core
- FDS.Hydraulics
- FDS.Thermal
- FDS.Airflow
- FDS.CFD
- FDS.IFC
- FDS.Revit
- FDS.Visualization

## Roadmap

### Phase 1

Hydraulischer Netzsolver

### Phase 2

Thermische Berechnung

### Phase 3

Lufttechnische Netze

### Phase 4

IFC- und Revit-Schnittstellen

### Phase 5

3D-Strömungssimulation

## Entwicklung

```bash
dotnet restore FluidDynamicsSimulator.sln
dotnet build FluidDynamicsSimulator.sln
dotnet test FluidDynamicsSimulator.sln
```

## Lizenz

MIT License
