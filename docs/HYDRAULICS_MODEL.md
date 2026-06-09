# FDS.Hydraulics Modellgrundlage

Stand: 2026-06-09

`FDS.Hydraulics` enthält erste hydraulische Einzelrohr- und Einzelwiderstands-Bausteine. Das Modul berechnet lokale Kennwerte für ein Rohr, Armaturen und Ventile, löst aber kein Netzwerk.

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

## Grenzen

- Kein kompletter hydraulischer Netzwerksolver
- Keine Pumpenkennlinie
- Keine Regelventil-Auslegung
- Keine Regelungstechnik
- Keine UI
- Keine IFC- oder Revit-Schnittstelle
