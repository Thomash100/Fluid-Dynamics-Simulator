# Solvervalidierung

Stand: 2026-06-27

## Aktueller Solverstatus

`FDS.Hydraulics` enthält:

- Einzelrohrberechnungen.
- Zeta- und Kv/Kvs-basierte Einzelwiderstände.
- Pumpenkennlinien und Pumpenleistung.
- Strangberechnung bei bekanntem Volumenstrom.
- Feste Netzwerkauswertung bei bekannten Strangvolumenströmen.
- Residualvorbereitung für Knotenbilanzen und Druckgleichungen.
- Einen kleinen iterativen Referenzsolver für vorbereitete Netze.

Der vorhandene Solver ist ein Referenzsolver für kleine vorbereitete Netze. Er ist kein allgemeiner hydraulischer Netzwerksolver.

## Residualkonventionen

Knotenbilanz:

```text
Q_res = Q_in + Q_source - Q_out - Q_sink
```

Druckresidual:

```text
dp_res = dp_available - dp_required
```

Konvergenz wird nur akzeptiert, wenn Flow-Residual und Druck-Residual innerhalb der Toleranzen aus `HydraulicSolverOptions` liegen.

## Referenzannahmen

Die Referenzfälle in `tests/FDS.Hydraulics.Tests/ReferenceCases/HydraulicSolverReferenceCases.cs` verwenden bewusst kleine, analytisch nachvollziehbare Netze:

- Fluid: Wasser mit `rho = 1000 kg/m3`.
- Dynamische Viskosität: `0,001 Pa*s`.
- Rohrlänge: `0 m`, damit keine Darcy-Weisbach-Rohrreibung in die Referenzwerte eingeht.
- Rohrinnendurchmesser: `0,1 m`, Querschnitt `A = 0,00785398163397448 m2`.
- Druckverluste entstehen in den Hauptfällen nur über Zeta-Widerstände:

```text
dp = zeta * rho * v^2 / 2
```

Damit ergeben sich für `v = 2 m/s` und `zeta = 1` genau `2000 Pa`. Für den zweiten Parallelstrang ergeben `v = 1 m/s` und `zeta = 4` ebenfalls `2000 Pa`.

## Validierungsfälle

| Fall | Testdaten-ID | Erwartung |
| --- | --- | --- |
| Einstrangnetz mit bekannter Druckdifferenz | `single-branch-known-pressure-difference` | Status `Converged`; `Q = 0,015707963267949 m3/s`; Knotenbilanz-Residual `0 m3/s`; Druckresidual `0 Pa` bei `dp_available = dp_required = 2000 Pa`. |
| Zwei parallele Stränge | `two-parallel-branches` | Status `Converged`; `Q_low = 0,015707963267949 m3/s`, `Q_high = 0,00785398163397448 m3/s`; beide Stränge haben Druckresidual `0 Pa`; Gesamt-Knotenbilanz `0 m3/s`. |
| Feste Pumpendruckerhöhung | `fixed-pump-pressure-increase` | Status `Converged`; konstante Pumpenkennlinie mit `H = 0,203943242595586 m`; Druckangebot `2000 Pa`; Druckresidual `0 Pa`. |
| Zu kleine Iterationszahl | `max-iterations-reached` | Status `MaxIterationsReached`; `Iterations = 1`; Knotenbilanz- und Druckresidual bleiben größer als `0`. |
| Ungültige Eingabe | `invalid-input` | Status `InvalidInput`; keine Iterationshistorie; keine verdeckte Ausnahme. |
| Nullfluss-Grenzfall | `zero-flow-boundary-case` | Status `Converged`; `Q = 0 m3/s`; Knotenbilanz-Residual `0 m3/s`; Druckresidual `0 Pa`; kein Division-by-zero. |

## Teststruktur

Die Referenzfälle sind getrennt von den Assertions abgelegt:

- `tests/FDS.Hydraulics.Tests/ReferenceCases/HydraulicSolverReferenceCase.cs`
- `tests/FDS.Hydraulics.Tests/ReferenceCases/HydraulicSolverReferenceCases.cs`
- `tests/FDS.Hydraulics.Tests/HydraulicSolverReferenceCaseTests.cs`

Die Testdaten beschreiben Eingaben, erwarteten Solverstatus, erwartete Branch-Flüsse, Knotenbilanz-Residuen und Druckresiduen. Die Assertions prüfen diese Werte gegen `SmallHydraulicNetworkSolver`, ohne Solverlogik oder Modelle zu ändern.

## Grenzen

- Kein Newton-Verfahren.
- Kein Hardy-Cross-Verfahren als allgemeiner Solver.
- Kein Gradient-Solver.
- Kein allgemeiner Netzwerksolver.
- Keine automatische Pumpenauswahl.
- Keine Regelstrategie.
- Keine thermische Kopplung.
- Keine IFC-/BIM-Anbindung.

