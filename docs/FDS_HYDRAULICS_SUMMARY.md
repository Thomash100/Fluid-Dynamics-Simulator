# Abschlusszusammenfassung FDS.Hydraulics Bausteine

Stand: 2026-06-12

## Ergebnis

`FDS.Hydraulics` ist als eigenes .NET 8 Modul implementiert und referenziert `FDS.Core`.

Die Solution `FluidDynamicsSimulator.sln` enthält jetzt:

- `src/FDS.Core`
- `src/FDS.Hydraulics`
- `tests/FDS.Core.Tests`
- `tests/FDS.Hydraulics.Tests`

## Umgesetzter Umfang

- Projekt `FDS.Hydraulics`
- Testprojekt `FDS.Hydraulics.Tests`
- Referenz von `FDS.Hydraulics` auf `FDS.Core`
- Rohrmodell `Pipe`
- Strömungsgeschwindigkeit in m/s
- Reynoldszahl
- Darcy-Reibungszahl mit laminarer Formel und einfacher Blasius-Näherung
- Darcy-Weisbach-Druckverlust als Einzelrohr-Hilfsrechnung
- Modell `LocalResistance`
- Modell `Fitting`
- Modell `Valve`
- Modell `ValveFlowCoefficient`
- Zeta-Wert-basierter Druckverlust
- Kv/Kvs-Grundmodell für Ventile
- Pumpenmodell `Pump`
- Pumpenkennlinie `PumpCurve`
- Kennlinien-Stützpunkte `PumpCurvePoint`
- Optionale Wirkungsgradkennlinie `PumpEfficiencyCurve`
- Wirkungsgrad-Stützpunkte `PumpEfficiencyPoint`
- Lineare Förderhöheninterpolation bei gegebenem Volumenstrom
- Hydraulische Pumpenleistung
- Optionale Wellenleistung aus Wirkungsgradkennlinie
- Pumpen-Druckerhöhung in Pa
- Modell `HydraulicBranch`
- Ergebnisobjekt `HydraulicBranchResult`
- Strangberechnung `HydraulicBranchCalculator`
- Aggregation von Rohrdruckverlusten, Einzelwiderständen und Pumpen-Druckerhöhung bei vorgegebenem Volumenstrom
- Modell `HydraulicBranchFlow`
- Modell `HydraulicNetwork`
- Ergebnisobjekte `HydraulicNetworkBranchResult` und `HydraulicNetworkResult`
- Netzwerkauswertung `HydraulicNetworkCalculator`
- Ermittlung des ungünstigsten Strangs
- Erforderliche Mindest-Pumpendruckerhöhung in Pa
- Optionale erforderliche Förderhöhe in m bei positiver Fluiddichte
- Solver-Optionen `HydraulicSolverOptions`
- Solver-Status `HydraulicSolverStatus`
- Randbedingungen `HydraulicBoundaryCondition`
- Knotenbilanz `HydraulicNodeBalance`
- Druckresidual `HydraulicPressureResidual`
- Solver-Vorbereitungsergebnis `HydraulicSolverResult`
- Residualvorbereitung `HydraulicSolverPreparationCalculator`
- Solver-Schnittstelle `IHydraulicNetworkSolver`
- Solver-Eingabe `HydraulicSolverInput`
- Iterationssnapshot `HydraulicSolverIteration`
- Kleiner Referenzsolver `SmallHydraulicNetworkSolver`
- Status `MaxIterationsReached`
- Unit Tests für Rohrmodell, Einzelwiderstände, Ventile, Pumpen, Strangberechnung, Netzwerkauswertung, Solver-Vorbereitung und kleinen Referenzsolver

## Nicht umgesetzt

- Kein allgemeiner Netzwerksolver
- Kein automatischer Volumenstromabgleich
- Keine automatische Pumpen-Betriebspunktberechnung
- Keine Newton-, Hardy-Cross- oder Gradient-Iteration
- Keine Pumpenkennlinienauswahl
- Keine Pumpenregelstrategie
- Keine Regelventil-Auslegung
- Keine UI

## Verifikation

```text
dotnet test FluidDynamicsSimulator.sln
```

Ergebnis: 119 Tests bestanden.
