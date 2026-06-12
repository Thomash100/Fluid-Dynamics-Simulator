# FDS.Hydraulics Modellgrundlage

Stand: 2026-06-12

`FDS.Hydraulics` enthält erste hydraulische Einzelrohr-, Einzelwiderstands-, Pumpen-, Strang-, Netzwerkauswertungs- und Solver-Vorbereitungsbausteine. Das Modul berechnet lokale Kennwerte für ein Rohr, Armaturen, Ventile, eine einzelne Pumpe, einen einfachen Strang und mehrere bekannte Stränge bei vorgegebenen Volumenströmen. Zusätzlich werden Knotenbilanzen und Residualwerte für einen späteren iterativen Solver vorbereitet. Ein vollständiger Netzwerksolver ist noch nicht enthalten.

## Enthalten

| Baustein | Verantwortung |
| --- | --- |
| `Pipe` | Rohrsegment mit Länge, Innendurchmesser, optionalen Knotenreferenzen und Rauheit. |
| `PipeFlowCalculator` | Reine statische Einzelrohr-Hilfsrechnungen. |
| `LocalResistance` | Generischer Einzelwiderstand mit dimensionslosem Zeta-Wert. |
| `Fitting` | Formstück-/Armaturenmodell auf Basis eines Zeta-Werts. |
| `Valve` | Ventil-Grundmodell mit optionalem Zeta-Widerstand und optionalem Kv/Kvs-Datensatz. |
| `ValveFlowCoefficient` | Kv/Kvs-Werte in m³/h. |
| `LocalResistanceCalculator` | Zeta- und Kv-basierte Einzelkomponenten-Hilfsrechnungen. |
| `Pump` | Pumpen-Grundmodell mit Förderhöhenkennlinie und optionaler Wirkungsgradkennlinie. |
| `PumpCurve` | Förderhöhenkennlinie aus Volumenstrom/Förderhöhe-Stützpunkten. |
| `PumpEfficiencyCurve` | Optionale Wirkungsgradkennlinie aus Volumenstrom/Wirkungsgrad-Stützpunkten. |
| `PumpCalculator` | Einzelpumpen-Hilfsrechnungen für Kennlinie und Leistung. |
| `HydraulicBranch` | Einfacher hydraulischer Strang aus Rohren, lokalen Widerständen, Armaturen, Ventilen und optionaler Pumpe. |
| `HydraulicBranchResult` | Ergebnisobjekt für Druckverlust, Pumpendruckerhöhung und Netto-Druckbilanz. |
| `HydraulicBranchCalculator` | Aggregiert Strangkomponenten bei einem vorgegebenen Volumenstrom. |
| `HydraulicBranchFlow` | Ordnet einem Strang einen bekannten nichtnegativen Volumenstrom zu. |
| `HydraulicNetwork` | Feste Netzwerkauswertung aus mehreren Strängen mit bekannten Volumenströmen. |
| `HydraulicNetworkBranchResult` | Netzwerkbezogenes Ergebnis für einen einzelnen Strang. |
| `HydraulicNetworkResult` | Gesamtergebnis mit BranchResults, kritischem Strang und erforderlicher Pumpendruckerhöhung. |
| `HydraulicNetworkCalculator` | Wertet mehrere Stränge aus und bestimmt die loss-basierte Mindest-Pumpendruckerhöhung. |
| `HydraulicSolverOptions` | Optionen für spätere iterative Solverläufe, inklusive Toleranzen und Relaxationsfaktor. |
| `HydraulicSolverStatus` | Statuswerte für Solver-Vorbereitung und spätere Solverläufe. |
| `HydraulicBoundaryCondition` | Randbedingungen für Quelle, Senke, bekannten Druck, bekannte Druckdifferenz oder Pumpenkennlinie. |
| `HydraulicNodeBalance` | Knotenbilanz mit Einströmung, Ausströmung, Quelle, Senke und Residualwert. |
| `HydraulicPressureResidual` | Druckresidual für spätere Kanten- oder Stranggleichungen. |
| `HydraulicSolverResult` | Ergebnis der Solver-Vorbereitung mit Knotenbilanzen und Druckresiduen. |
| `HydraulicSolverPreparationCalculator` | Bereitet Residualdaten ohne Iteration oder Solverentscheidung vor. |

## Berechnungen

| Berechnung | Einheit Ergebnis | Methode |
| --- | --- | --- |
| Querschnittsfläche | m² | `CalculateCrossSectionalAreaSquareMeters` |
| Strömungsgeschwindigkeit | m/s | `CalculateVelocityMetersPerSecond` |
| Reynoldszahl | dimensionslos | `CalculateReynoldsNumber` |
| Darcy-Reibungszahl | dimensionslos | `EstimateDarcyFrictionFactor` |
| Darcy-Weisbach-Druckverlust | Pa | `CalculateDarcyWeisbachPressureLossPascals` |
| Zeta-Druckverlust | Pa | `CalculateZetaPressureLossPascals` |
| Kv-basierter Ventil-Druckverlust | Pa | `CalculateValvePressureLossFromKvPascals` |
| Interpolierte Pumpen-Förderhöhe | m | `InterpolateHeadMeters` |
| Hydraulische Pumpenleistung | W | `CalculateHydraulicPowerWatts` |
| Pumpen-Wellenleistung | W | `CalculateShaftPowerWatts` |
| Pumpen-Druckerhöhung | Pa | `CalculatePressureIncreasePascals` |
| Strang-Druckbilanz | Pa | `HydraulicBranchCalculator.Calculate` |
| Netzwerkauswertung | Pa und m | `HydraulicNetworkCalculator.Calculate` |
| Knotenbilanz | m³/s | `HydraulicSolverPreparationCalculator.Prepare` |
| Druckresidual | Pa | `HydraulicSolverPreparationCalculator.Prepare` |

## Einheiten

| Größe | Einheit |
| --- | --- |
| Rohrlänge | m |
| Rohrinnendurchmesser | m |
| Rohrrauheit | m |
| Volumenstrom | m³/s |
| Dichte | kg/m³ |
| Dynamische Viskosität | Pa·s |
| Geschwindigkeit | m/s |
| Druckverlust | Pa |
| Zeta-Wert | dimensionslos |
| Kv/Kvs | m³/h |
| Pumpen-Förderhöhe | m |
| Pumpenleistung | W |
| Wirkungsgrad | dimensionslos, 0 < eta <= 1 |
| Netto-Druckbilanz | Pa |
| Erforderliche Pumpendruckerhöhung | Pa |
| Erforderliche Förderhöhe | m |
| Knotenbilanz-Residual | m³/s |
| Druck-Residual | Pa |

## Reibungszahl-Modell

- Laminar: `f = 64 / Re`
- Nicht-laminar: einfache Blasius-Näherung `f = 0.3164 / Re^0.25`

Die Blasius-Näherung ist ein bewusst einfacher Platzhalter für glatte Rohre. Sie ersetzt noch kein vollständiges Rauheits-, Übergangs- oder Colebrook-Modell.

## Einzelwiderstände und Armaturen

Zeta-basierter Druckverlust wird als positive Verlustgröße berechnet:

```text
dp = zeta * rho * v² / 2
```

Dabei ist `rho` die Dichte in kg/m³ und `v` die Geschwindigkeit in m/s. Negative Flussrichtung wird als Betrag behandelt.

## Kv/Kvs-Grundmodell

Kv und Kvs werden in m³/h gespeichert. Die vorbereitete Ventilrechnung nutzt die metrische Kv-Konvention:

```text
dp_bar = (rho / 1000) * (Q_m3h / Kv)²
```

Das ist keine Regelventil-Auslegung und keine Ventilautoritätsberechnung. Es ist nur ein Basismodell für spätere Ventilbausteine.

## Pumpenmodell

`PumpCurve` speichert Stützpunkte aus Volumenstrom `Q` in m³/s und Förderhöhe `H` in m. Die Förderhöhe wird innerhalb des Kennlinienbereichs linear interpoliert. Werte außerhalb des Stützpunktbereichs werden bewusst abgelehnt, damit keine stillschweigende Extrapolation entsteht.

Die hydraulische Leistung wird für einen vorgegebenen Volumenstrom berechnet:

```text
P_h = rho * g * Q * H
```

Dabei ist `rho` die Dichte in kg/m³, `g` die Erdbeschleunigung in m/s², `Q` der Volumenstrom in m³/s und `H` die interpolierte Förderhöhe in m.

Optional kann `PumpEfficiencyCurve` eine Wirkungsgradkennlinie hinterlegen. Daraus wird die Wellenleistung berechnet:

```text
P_shaft = P_h / eta
```

Das Pumpenmodell selbst berechnet keinen automatischen Betriebspunkt. Die einfache Strangberechnung kann Pumpen-Druckerhöhung und Komponentenverluste bei einem vorgegebenen Volumenstrom aggregieren.

## Strangberechnung

`HydraulicBranch` fasst eine einfache Strangtopologie zusammen:

- Rohre
- generische Einzelwiderstände
- Armaturen/Formstücke
- Ventile
- optionale Pumpe

`HydraulicBranchCalculator.Calculate` wertet den Strang für einen vorgegebenen, nichtnegativen Volumenstrom aus. Das Ergebnis enthält:

- Volumenstrom in m³/s
- Summe Rohrdruckverluste in Pa
- Summe Einzelwiderstände in Pa
- Pumpen-Druckerhöhung in Pa
- Netto-Druckbilanz in Pa

Die Netto-Druckbilanz wird berechnet als:

```text
dp_net = dp_pump - dp_pipe - dp_local
```

Positive Werte bedeuten, dass die Pumpen-Druckerhöhung die aggregierten Verluste übersteigt. Negative Werte bedeuten, dass die Verluste größer als die Pumpen-Druckerhöhung sind.

Zeta-basierte Einzelwiderstände benötigen eine Bezugsgeschwindigkeit. In diesem ersten einfachen Strangmodell wird dafür `HydraulicBranch.LocalResistanceReferencePipe` verwendet. Standardmäßig ist das die erste Pipe des Strangs. Eine spätere Segmentmodellierung kann Einzelwiderstände bauteilscharf einzelnen Rohrabschnitten zuordnen.

Ventile mit `ValveFlowCoefficient` werden in der Strangberechnung über Kv berechnet. Ist kein Kv/Kvs-Datensatz vorhanden, wird der optionale Zeta-Widerstand verwendet.

## Feste Netzwerkauswertung

`HydraulicNetwork` fasst mehrere `HydraulicBranchFlow`-Einträge zusammen. Jeder Eintrag enthält einen bereits definierten `HydraulicBranch` und einen bekannten nichtnegativen Volumenstrom in m³/s. Der Volumenstrom wird nicht berechnet oder verteilt.

`HydraulicNetworkCalculator.Calculate` verwendet intern für jeden Strang `HydraulicBranchCalculator.Calculate`. Das Netzwerkergebnis enthält:

- alle `HydraulicNetworkBranchResult`-Einträge
- die Druckbilanz jedes Strangs über das enthaltene `HydraulicBranchResult`
- den ungünstigsten Strang als `CriticalBranchResult`
- die erforderliche Mindest-Pumpendruckerhöhung in Pa
- optional die erforderliche Förderhöhe in m, wenn die Fluiddichte größer als 0 ist

Die erforderliche Mindest-Pumpendruckerhöhung wird aktuell loss-basiert bestimmt:

```text
dp_required = max(dp_pipe + dp_local)
```

Vorhandene optionale Pumpen in Strängen bleiben in der jeweiligen Netto-Druckbilanz sichtbar. Sie reduzieren die loss-basierte Mindest-Pumpendruckerhöhung für die vorbereitende Pumpenauswertung nicht.

## Solver-Vorbereitung

Die Solver-Vorbereitung ist bewusst keine iterative Netzwerklösung. Sie stellt Datenstrukturen und Residualauswertung bereit, die ein späterer Newton-, Hardy-Cross- oder Gradienten-Solver verwenden kann.

`HydraulicBoundaryCondition` unterstützt vorbereitend:

- Quelle mit bekanntem Volumenstrom
- Senke mit bekanntem Volumenstrom
- bekannten Knotendruck
- bekannte Druckdifferenz zwischen zwei Knoten
- Pumpenkennlinie zwischen zwei Knoten

`HydraulicSolverPreparationCalculator.Prepare` kann auf einer `FDS.Core.Network`-Topologie Knotenbilanzen aus vorhandenen Kanten-Volumenströmen berechnen. Die Residualkonvention lautet:

```text
Q_res = Q_in + Q_source - Q_out - Q_sink
```

Zusätzlich kann eine vorhandene feste `HydraulicNetwork`-Auswertung eingebunden werden. Daraus werden Druckresiduen je Strang erzeugt:

```text
dp_res = dp_available - dp_required
```

Dabei ist `dp_available` die vorhandene Pumpen-Druckerhöhung im Strang und `dp_required` die Summe aus Rohr- und Einzelwiderstandsverlusten.

Der Status eines vorbereiteten Ergebnisses ist `Prepared`, die Iterationszahl bleibt `0`.

## Grenzen

- Kein kompletter hydraulischer Netzwerksolver
- Kein automatischer Volumenstromabgleich
- Keine automatische Pumpen-Betriebspunktberechnung
- Keine Newton-, Hardy-Cross- oder Gradient-Iteration
- Keine iterative Stranglösung
- Keine Pumpenkennlinienauswahl
- Keine Pumpenregelstrategie
- Keine Regelventil-Auslegung
- Keine Regelungstechnik
- Keine UI
- Keine IFC- oder Revit-Schnittstelle
