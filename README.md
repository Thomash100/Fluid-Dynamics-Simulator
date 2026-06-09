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

`FDS.Hydraulics` ist als nächstes Modul angelegt. Es enthält ein Rohrmodell und Einzelrohr-Berechnungen für Strömungsgeschwindigkeit, Reynoldszahl, einfache Reibungszahl-Abschätzung und vorbereiteten Darcy-Weisbach-Druckverlust. Es ist noch kein kompletter Netzwerksolver implementiert.

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
      PipeFlowCalculator.cs
    Models/
      Pipe.cs

tests/
  FDS.Core.Tests/
  FDS.Hydraulics.Tests/
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

## Nicht enthalten

- Kein kompletter hydraulischer Netzwerksolver
- Keine Pumpenkennlinie
- Keine Ventilberechnung
- Keine UI
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
