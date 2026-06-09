# FDS.Hydraulics Modellgrundlage

Stand: 2026-06-09

`FDS.Hydraulics` enthält erste hydraulische Einzelrohr-Bausteine. Das Modul berechnet lokale Kennwerte für ein Rohr, löst aber kein Netzwerk.

## Enthalten

| Baustein | Verantwortung |
| --- | --- |
| `Pipe` | Rohrsegment mit Länge, Innendurchmesser, optionalen Knotenreferenzen und Rauheit. |
| `PipeFlowCalculator` | Reine statische Einzelrohr-Hilfsrechnungen. |

## Berechnungen

| Berechnung | Einheit Ergebnis | Methode |
| --- | --- | --- |
| Querschnittsfläche | m² | `CalculateCrossSectionalAreaSquareMeters` |
| Strömungsgeschwindigkeit | m/s | `CalculateVelocityMetersPerSecond` |
| Reynoldszahl | dimensionslos | `CalculateReynoldsNumber` |
| Darcy-Reibungszahl | dimensionslos | `EstimateDarcyFrictionFactor` |
| Darcy-Weisbach-Druckverlust | Pa | `CalculateDarcyWeisbachPressureLossPascals` |

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

## Reibungszahl-Modell

- Laminar: `f = 64 / Re`
- Nicht-laminar: einfache Blasius-Näherung `f = 0.3164 / Re^0.25`

Die Blasius-Näherung ist ein bewusst einfacher Platzhalter für glatte Rohre. Sie ersetzt noch kein vollständiges Rauheits-, Übergangs- oder Colebrook-Modell.

## Grenzen

- Kein kompletter hydraulischer Netzwerksolver
- Keine Pumpenkennlinie
- Keine Ventilberechnung
- Keine Regelungstechnik
- Keine UI
- Keine IFC- oder Revit-Schnittstelle
